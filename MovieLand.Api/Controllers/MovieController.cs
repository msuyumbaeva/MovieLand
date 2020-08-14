using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MovieLand.Api.Models;
using MovieLand.BLL;
using MovieLand.BLL.Configurations;
using MovieLand.BLL.Contracts;
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
        private readonly IFileClient _fileClient;
        private readonly MoviePosterFileConfiguration _fileConfiguration;

        public MovieController(IMovieService movieService, IFileClient fileClient, IOptions<MoviePosterFileConfiguration> fileConfiguration) {
            _movieService = movieService ?? throw new ArgumentNullException(nameof(movieService));
            _fileClient = fileClient ?? throw new ArgumentNullException(nameof(fileClient));
            _fileConfiguration = fileConfiguration?.Value ?? throw new ArgumentNullException(nameof(fileConfiguration));
        }

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

        // GET: api/Movie/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<IActionResult> Get(Guid id) {
            var movieResult = await _movieService.GetByIdAsync(id);
            if (movieResult.Entity == null)
                return NotFound();
            else
                return Ok(movieResult.Entity);
        }
        
    }
}
