using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MovieLand.Api.Models;
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
        public async Task<IActionResult> Get(string search, int start = 0, int length = 10) {
            var param = new DataTablesParameters {
                Columns = new DTColumn[1] {
                    new DTColumn() { Data = "Name", Name = "Name", Orderable = true, Searchable = true }
                },
                Order = new DTOrder[1] {
                    new DTOrder() { Column = 0, Dir = DTOrderDir.ASC }
                },
                Draw = 0,
                Start = start,
                Length = length,
                Search = new DTSearch() {
                    Value = search ?? "",
                    Regex = false
                }
            };

            var result = await _movieService.GetAsync(param);
            if (result.IsSuccess) {
                return Ok(new DataTablesResult<MovieListItemDto> {
                    Draw = param.Draw,
                    Data = result.Entity.Items,
                    RecordsFiltered = result.Entity.Items.Count(),
                    RecordsTotal = result.Entity.TotalSize
                });
            }
            return StatusCode(500, "Internal server error");
        }

        // GET: api/Movie/Genre/{id}
        [HttpGet]
        [Route("[action]/{id}")]
        public async Task<IActionResult> Genre(Guid id, int start = 0, int length = 10) {
            var result = await _movieService.GetByGenreAsync(id, length, start);
            if (result.IsSuccess) {
                return Ok(new DataTablesResult<MovieListItemDto> {
                    Draw = 0,
                    Data = result.Entity.Items,
                    RecordsFiltered = result.Entity.Items.Count(),
                    RecordsTotal = result.Entity.TotalSize
                });
            }
            return StatusCode(500, "Internal server error");
        }

        // GET: api/Movie/Country/{id}
        [HttpGet]
        [Route("[action]/{id}")]
        public async Task<IActionResult> Country(Guid id, int start = 0, int length = 10) {
            var result = await _movieService.GetByCountryAsync(id, length, start);
            if (result.IsSuccess) {
                return Ok(new DataTablesResult<MovieListItemDto> {
                    Draw = 0,
                    Data = result.Entity.Items,
                    RecordsFiltered = result.Entity.Items.Count(),
                    RecordsTotal = result.Entity.TotalSize
                });
            }
            return StatusCode(500, "Internal server error");
        }

        // GET: api/Movie/Artist/{id}
        [HttpGet]
        [Route("[action]/{id}")]
        public async Task<IActionResult> Artist(Guid id, int start = 0, int length = 10) {
            var result = await _movieService.GetByArtistAsync(id, length, start);
            if (result.IsSuccess) {
                return Ok(new DataTablesResult<MovieListItemDto> {
                    Draw = 0,
                    Data = result.Entity.Items,
                    RecordsFiltered = result.Entity.Items.Count(),
                    RecordsTotal = result.Entity.TotalSize
                });
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
