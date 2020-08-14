using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MovieLand.Api.Models;
using MovieLand.Api.Models.Comment;
using MovieLand.BLL;
using MovieLand.BLL.Configurations;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Dtos.Comment;
using MovieLand.BLL.Dtos.DataTables;
using MovieLand.BLL.Dtos.Movie;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MovieLand.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly ICommentService _commentService;
        private readonly IFileClient _fileClient;
        private readonly MoviePosterFileConfiguration _fileConfiguration;

        public MovieController(IMovieService movieService, ICommentService commentService, IFileClient fileClient, IOptions<MoviePosterFileConfiguration> fileConfiguration) {
            _movieService = movieService ?? throw new ArgumentNullException(nameof(movieService));
            _commentService = commentService ?? throw new ArgumentNullException(nameof(commentService));
            _fileClient = fileClient ?? throw new ArgumentNullException(nameof(fileClient));
            _fileConfiguration = fileConfiguration?.Value ?? throw new ArgumentNullException(nameof(fileConfiguration));
        }

        #region Genre endpoints
        // GET: api/Movie
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]MovieFilterParameters filters, [FromQuery]PaginationParameters pagination) {
            var param = new MovieDataTablesParameters {
                Columns = new DTColumn[1] {
                    new DTColumn() { Data = "Name", Name = "Name", Orderable = true, Searchable = true }
                },
                Order = new DTOrder[1] {
                    new DTOrder() { Column = 0, Dir = DTOrderDir.ASC }
                },
                Draw = 0,
                Start = pagination.Offset,
                Length = pagination.Limit,
                Search = new DTSearch() {
                    Value = filters.Search ?? "",
                    Regex = false
                },
                Genre = filters.Genre,
                Country = filters.Country,
                Artist = filters.Artist
            };
            var result = await _movieService.GetAsync(param);

            if (result.IsSuccess) {
                return Ok(new ArrayResult<MovieListItemDto>(
                    result.Entity.Items
                ));
            }
            return StatusCode(500, "Internal server error");
        }

        // GET: api/Movie/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<IActionResult> Get(Guid id) {
            var movieResult = await _movieService.GetByIdAsync(id);
            if (movieResult.Entity == null)
                return NotFound();
            else
                return Ok(movieResult.Entity);
        }
        #endregion Genre endpoints

        #region Poster endpoints
        // GET: api/Movie/Poster/5
        [HttpGet]
        [Route("[action]/{id}")]
        public async Task<IActionResult> Poster(Guid id) {
            var movieResult = await _movieService.GetByIdAsync(id);
            if (!movieResult.IsSuccess)
                return StatusCode(500, "Internal server error");

            if (movieResult.Entity == null)
                return NotFound();

            var fileStream = _fileClient.GetFile(_fileConfiguration.Directory, movieResult.Entity.Poster);
            return File(fileStream, "image/jpeg");
        }
        #endregion Poster endpoints

        #region Comments endpoints
        [HttpGet]
        [Route("{id}/[action]")]
        public async Task<IActionResult> Comments(Guid id, [FromQuery] PaginationParameters pagination) {
            var param = new DataTablesParameters {
                Draw = 0,
                Start = pagination.Offset,
                Length = pagination.Limit
            };
            var result = await _commentService.GetByMovieIdAsync(id, param);
            if (result.IsSuccess) {
                return Ok(new ArrayResult<CommentDto>(
                    result.Entity.Items
                ));
            }
            return StatusCode(500, "Internal server error");
        }

        [HttpPost]
        [Route("{id}/[action]")]
        [ActionName("Comments")]
        [Authorize(Roles = "USER")]
        public async Task<IActionResult> CreateComment(Guid id, [FromBody] CommentCreateRequest request) {
            if (ModelState.IsValid) {
                var commentDto = new CommentDto() {
                    MovieId = id,
                    Text = request.Text,
                    UserName = User.Identity.Name
                };
                var result = await _commentService.CreateAsync(commentDto);
                if (!result.IsSuccess) {
                    return BadRequest(result.Errors);
                }
                return StatusCode(201);
            }
            return BadRequest();
        }
        #endregion Comments endpoints
    }
}
