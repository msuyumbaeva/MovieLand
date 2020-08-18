using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MovieLand.Api.HyperMedia;
using MovieLand.Api.Models;
using MovieLand.Api.Models.Comment;
using MovieLand.Api.Models.StarRating;
using MovieLand.BLL;
using MovieLand.BLL.Configurations;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Dtos.Comment;
using MovieLand.BLL.Dtos.DataTables;
using MovieLand.BLL.Dtos.Movie;
using MovieLand.BLL.Dtos.StarRating;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MovieLand.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly ICommentService _commentService;
        private readonly IStarRatingService _starRatingService;
        private readonly IFileClient _fileClient;
        private readonly MoviePosterFileConfiguration _fileConfiguration;

        public MoviesController(IMovieService movieService, ICommentService commentService, IStarRatingService starRatingService, IFileClient fileClient, IOptions<MoviePosterFileConfiguration> fileConfiguration) {
            _movieService = movieService ?? throw new ArgumentNullException(nameof(movieService));
            _commentService = commentService ?? throw new ArgumentNullException(nameof(commentService));
            _starRatingService = starRatingService ?? throw new ArgumentNullException(nameof(starRatingService));
            _fileClient = fileClient ?? throw new ArgumentNullException(nameof(fileClient));
            _fileConfiguration = fileConfiguration?.Value ?? throw new ArgumentNullException(nameof(fileConfiguration));
        }



        #region Movie endpoints
        // GET: api/Movie
        [HttpGet(Name = nameof(GetMovies))]
        [TypeFilter(typeof(HyperMediaFilter))]
        public async Task<IActionResult> GetMovies([FromQuery]MovieFilterParameters filters, [FromQuery]PaginationParameters pagination) {
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
                return Ok(new ResultSet<MovieListItemDto>(
                    result.Entity.Items,
                    new ResultSetMethadata(result.Entity.TotalSize, pagination.Limit, pagination.Offset)
                ));
            }
            return StatusCode((int)HttpStatusCode.InternalServerError, new { result.Errors });
        }

        // GET: api/Movie/5
        [HttpGet("{id}", Name = nameof(GetMovie))]
        [TypeFilter(typeof(HyperMediaFilter))]
        public async Task<IActionResult> GetMovie(Guid id) {
            var movieResult = await _movieService.GetByIdAsync(id);
            if (movieResult.Entity == null)
                return NotFound();
            else
                return Ok(new HyperMediaLinksDecorator<MovieDto>(movieResult.Entity));
        }
        #endregion Movie endpoints

        #region Poster endpoints
        // GET: api/Movie/Poster/5
        [HttpGet]
        [Route("{id}/[action]", Name = nameof(GetMoviePoster))]
        [ActionName("Poster")]
        public async Task<IActionResult> GetMoviePoster(Guid id) {
            var movieResult = await _movieService.GetByIdAsync(id);
            if (!movieResult.IsSuccess)
                return StatusCode((int)HttpStatusCode.InternalServerError, new { movieResult.Errors });

            if (movieResult.Entity == null)
                return NotFound();

            var fileStream = _fileClient.GetFile(_fileConfiguration.Directory, movieResult.Entity.Poster);
            return File(fileStream, "image/jpeg");
        }
        #endregion Poster endpoints

        #region Comments endpoints
        // GET: api/Movie/5/Comments
        [HttpGet]
        [ActionName("Comments")]
        [Route("{id}/[action]", Name = nameof(GetMovieComments))]
        [TypeFilter(typeof(HyperMediaFilter))]
        public async Task<IActionResult> GetMovieComments(Guid id, [FromQuery] PaginationParameters pagination) {
            var param = new DataTablesParameters {
                Draw = 0,
                Start = pagination.Offset,
                Length = pagination.Limit
            };
            var result = await _commentService.GetByMovieIdAsync(id, param);
            if (result.IsSuccess) {
                return Ok(new ResultSet<CommentDto>(
                    result.Entity.Items,
                    new ResultSetMethadata(result.Entity.TotalSize, pagination.Limit, pagination.Offset)
                ));
            }
            return StatusCode((int)HttpStatusCode.InternalServerError, new { result.Errors });
        }

        // POST: api/Movie/5/Comments
        [HttpPost]
        [Route("{id}/[action]", Name = nameof(PostMovieComment))]
        [ActionName("Comments")]
        [Authorize(Roles = "USER")]
        public async Task<IActionResult> PostMovieComment(Guid id, [FromBody] CommentCreateRequest request) {
            if (ModelState.IsValid) {
                var userId = User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                    return StatusCode((int)HttpStatusCode.Forbidden);

                var commentDto = new CommentDto() {
                    MovieId = id,
                    Text = request.Text,
                    User = userId
                };
                var result = await _commentService.CreateAsync(commentDto);
                if (!result.IsSuccess) {
                    return BadRequest(new { result.Errors });
                }
                return StatusCode((int)HttpStatusCode.Created);
            }
            return BadRequest();
        }
        #endregion Comments endpoints

        #region StarRating endpoints
        // POST: api/Movie/5/StarRating
        [HttpPost]
        [Route("{id}/[action]", Name = nameof(PostMovieStarRating))]
        [ActionName("StarRating")]
        [Authorize(Roles = "USER")]
        public async Task<IActionResult> PostMovieStarRating(Guid id, [FromBody] StarRatingCreateRequest request) {
            if (ModelState.IsValid) {
                var userId = User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                    return StatusCode((int)HttpStatusCode.Forbidden);

                var ratingDto = new StarRatingDto() {
                    MovieId = id,
                    Value = request.Value,
                    User = userId
                };
                var result = await _starRatingService.SaveAsync(ratingDto);
                if (!result.IsSuccess) {
                    return BadRequest(new { result.Errors });
                }
                return StatusCode((int)HttpStatusCode.Created);
            }
            return BadRequest();
        }

        // GET: api/Movie/5/StarRating
        [HttpGet]
        [Route("{id}/[action]", Name = nameof(GetMovieStarRating))]
        [ActionName("StarRating")]
        public async Task<IActionResult> GetMovieStarRating(Guid id) {
            var result = await _starRatingService.GetAverageRatingOfMovieAsync(id);
            if (!result.IsSuccess) {
                return BadRequest(new { result.Errors });
            }
            return Ok(new { Value = result.Entity });
        }

        #endregion StarRating endpoints
    }
}
