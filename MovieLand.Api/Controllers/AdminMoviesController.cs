using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovieLand.Api.HyperMedia;
using MovieLand.Api.Models;
using MovieLand.Api.Models.Movie;
using MovieLand.Api.Models.MovieSources;
using MovieLand.Api.Models.Omdb;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Dtos.Artist;
using MovieLand.BLL.Dtos.Country;
using MovieLand.BLL.Dtos.Genre;
using MovieLand.BLL.Dtos.Movie;
using MovieLand.Data.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MovieLand.Api.Controllers
{
    [Route("api/movies")]
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    public class AdminMoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly IGenreService _genreService;
        private readonly ICountryService _countryService;
        private readonly IArtistService _artistService;
        private readonly MovieSourceOptions _movieSourceOptions;
        private readonly IMapper _mapper;
        private static readonly HttpClient HttpClient = new HttpClient();
        private readonly ILogger<AdminMoviesController> _logger;

        public AdminMoviesController(IMovieService movieService, IGenreService genreService, ICountryService countryService, IArtistService artistService, MovieSourceOptions movieSourceOptions, IMapper mapper, ILogger<AdminMoviesController> logger) {
            _movieService = movieService ?? throw new ArgumentNullException(nameof(movieService));
            _genreService = genreService ?? throw new ArgumentNullException(nameof(genreService));
            _countryService = countryService ?? throw new ArgumentNullException(nameof(countryService));
            _artistService = artistService ?? throw new ArgumentNullException(nameof(artistService));
            _movieSourceOptions = movieSourceOptions ?? throw new ArgumentNullException(nameof(movieSourceOptions));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region Movies endpoints

        // POST: api/movies
        [HttpPost(Name = nameof(PostMovie))]
        [TypeFilter(typeof(HyperMediaFilter))]
        public async Task<IActionResult> PostMovie([FromForm] MovieCreateDto createDto) {
            _logger.LogInformation("Creating movie: {0}", createDto.Name);
            if (!ModelState.IsValid) {
                _logger.LogWarning("Validation failed: {0}", ModelState);
                return BadRequest();
            }

            _logger.LogInformation("Saving movie to database");
            var result = await _movieService.SaveAsync(createDto);

            if (!result.IsSuccess) {
                _logger.LogError(result.Errors);
                return BadRequest(new { result.Errors });
            }

            _logger.LogInformation("Created successfully");
            var response = new HyperMediaLinksDecorator<MovieDto>(result.Entity);
            EnrichMovieDtoResponse(response);
            return StatusCode((int)HttpStatusCode.Created, response);
        }

        [HttpGet]
        [Route("[action]", Name = nameof(GetMoviesFromExternalSource))]
        [ActionName("External")]
        public async Task<IActionResult> GetMoviesFromExternalSource([FromQuery] MovieSourceRequest request, int page = 1) {
            _logger.LogInformation("Get movie from external source: {0}", request.Source);

            // Find source from list
            var source = _movieSourceOptions.MovieSourcesList.FirstOrDefault(m => m.Name == request.Source);
            if (source == null) {
                _logger.LogWarning("Source {0} was not found", request.Source);
                return BadRequest("Source was not found");
            }
            try {
                // Get movies from source
                var movies = await source.SearchMovieAsync(HttpClient, request.Value, page);
                return Ok(movies);
            }
            catch (Exception ex) {
                _logger.LogError(ex.Message);
                return BadRequest(new { errors = new string[] { ex.Message } });
            }
        }

        [HttpPost]
        [Route("[action]",Name = nameof(PostMovieFromExternalSource))]
        [ActionName("External")]
        [TypeFilter(typeof(HyperMediaFilter))]
        public async Task<IActionResult> PostMovieFromExternalSource([FromBody] MovieSourceRequest request) {
            _logger.LogInformation("Creating movie from external source: {0}", request.Source);

            // Find source from list
            var source = _movieSourceOptions.MovieSourcesList.FirstOrDefault(m => m.Name == request.Source);
            if (source == null) {
                _logger.LogWarning("Source {0} was not found", request.Source);
                return BadRequest("Source was not found");
            }

            try {
                // Get movie from source
                _logger.LogInformation("Get movie request");
                var movieSourceDto = await source.GetMovieAsync(HttpClient, request.Value);

                // Map movie from source to MovieCreateDto object
                var createDto = _mapper.Map<MovieCreateDto>(movieSourceDto);
                // Save movie to db
                _logger.LogInformation("Save movie to database");
                var movieResult = await _movieService.SaveAsync(createDto);
                if (!movieResult.IsSuccess)
                    throw new Exception(movieResult.Errors.First());

                // Get created movie
                var createdMovie = movieResult.Entity;

                // Set movie genres
                _logger.LogInformation("Save movie genres");
                foreach(var genreName in movieSourceDto.Genres) {
                    // Get or create genre 
                    var genreResult = await _genreService.SaveAsync(new GenreDto() { Name = genreName });
                    if (!genreResult.IsSuccess)
                        throw new Exception(genreResult.Errors.First());

                    // Add genre to movie
                    var movieGenreResult = await _movieService.AddGenreAsync(createdMovie.Id, genreResult.Entity.Id);
                    if (!movieGenreResult.IsSuccess)
                        throw new Exception(movieGenreResult.Errors.First());

                    createdMovie.Genres.Add(genreResult.Entity);
                }

                // Set movie countries
                _logger.LogInformation("Save movie countries");
                foreach (var countryName in movieSourceDto.Countries) {
                    // Get or create country
                    var countryResult = await _countryService.SaveAsync(new CountryDto() { Name = countryName });
                    if (!countryResult.IsSuccess)
                        throw new Exception(countryResult.Errors.First());

                    // Add country to movie
                    var movieCountryResult = await _movieService.AddCountryAsync(createdMovie.Id, countryResult.Entity.Id);
                    if (!movieCountryResult.IsSuccess)
                        throw new Exception(movieCountryResult.Errors.First());

                    createdMovie.Countries.Add(countryResult.Entity);
                }

                // Set movie directors
                _logger.LogInformation("Save movie directors");
                byte priority = 1;
                foreach (var directorsName in movieSourceDto.Directors) {
                    // Get or create artist
                    var artistResult = await _artistService.SaveAsync(new ArtistDto() { Name = directorsName });
                    if (!artistResult.IsSuccess)
                        throw new Exception(artistResult.Errors.First());

                    // Add artist to movie
                    var movieArtistResult = await _movieService.SaveArtistAsync(createdMovie.Id, new MovieArtistDto(artistResult.Entity.Id, CareerEnum.Director, priority));
                    if (!movieArtistResult.IsSuccess)
                        throw new Exception(movieArtistResult.Errors.First());

                    createdMovie.Directors.Add(artistResult.Entity);
                    priority++;
                }

                // Set movie actors
                _logger.LogInformation("Save movie actors");
                priority = 1;
                foreach (var actorsName in movieSourceDto.Actors) {
                    // Get or create artist
                    var artistResult = await _artistService.SaveAsync(new ArtistDto() { Name = actorsName });
                    if (!artistResult.IsSuccess)
                        throw new Exception(artistResult.Errors.First());

                    // Add artist to movie
                    var movieArtistResult = await _movieService.SaveArtistAsync(createdMovie.Id, new MovieArtistDto(artistResult.Entity.Id, CareerEnum.Actor, priority));
                    if (!movieArtistResult.IsSuccess)
                        throw new Exception(movieArtistResult.Errors.First());

                    createdMovie.Actors.Add(artistResult.Entity);
                    priority++;
                }

                // Add Hypermedia links to response
                var response = new HyperMediaLinksDecorator<MovieDto>(createdMovie);
                EnrichMovieDtoResponse(response);
                return StatusCode((int)HttpStatusCode.Created, response);
            }
            catch (Exception ex) {
                _logger.LogError(ex.Message);
                return BadRequest(new { errors = new string[] { ex.Message } });
            }
        }

        #endregion Movies endpoints

        #region Genres endpoints

        // POST: api/movies/5/genres
        [HttpPost]
        [Route("{id}/[action]", Name = nameof(PostMovieGenre))]
        [ActionName("Genres")]
        public async Task<IActionResult> PostMovieGenre(Guid id, [FromBody] MovieGenreRequest request) {
            _logger.LogInformation("Adding genre {0} to movie {1}", request.GenreId, id);
            if (!ModelState.IsValid) {
                _logger.LogWarning("Validation failed: {0}", ModelState);
                return BadRequest();
            }

            var result = await _movieService.AddGenreAsync(id, request.GenreId);
            if (!result.IsSuccess) {
                _logger.LogError(result.Errors);
                return BadRequest(new { result.Errors });
            }

            var response = new LinkedResourceDto();
            EnrichMovieGenreResponse(id, request.GenreId, response);
            return StatusCode((int)HttpStatusCode.Created, response);
        }

        // DELETE: api/movies/5/genres/3
        [HttpDelete]
        [Route("{id}/[action]/{genreId}", Name = nameof(DeleteMovieGenre))]
        [ActionName("Genres")]
        public async Task<IActionResult> DeleteMovieGenre(Guid id, Guid genreId) {
            _logger.LogInformation("Deleting genre {0} from movie {1}", genreId, id);
            var result = await _movieService.RemoveGenreAsync(id, genreId);
            if (!result.IsSuccess) {
                _logger.LogError(result.Errors);
                return BadRequest(new { result.Errors });
            }
            return NoContent();
        }

        #endregion Genres endpoints

        #region Countries endpoints

        // POST: api/movies/5/countries
        [HttpPost]
        [Route("{id}/[action]", Name = nameof(PostMovieCountry))]
        [ActionName("Countries")]
        public async Task<IActionResult> PostMovieCountry(Guid id, [FromBody] MovieCountryRequest request) {
            _logger.LogInformation("Adding country {0} to movie {1}", request.CountryId, id);
            if (!ModelState.IsValid) {
                _logger.LogWarning("Validation failed: {0}", ModelState);
                return BadRequest();
            }

            var result = await _movieService.AddCountryAsync(id, request.CountryId);
            if (!result.IsSuccess) {
                _logger.LogError(result.Errors);
                return BadRequest(new { result.Errors });
            }

            var response = new LinkedResourceDto();
            EnrichMovieCountryResponse(id, request.CountryId, response);
            return StatusCode((int)HttpStatusCode.Created, response);
        }

        // DELETE: api/movies/5/countries/3
        [HttpDelete]
        [Route("{id}/[action]/{countryId}", Name = nameof(DeleteMovieCountry))]
        [ActionName("Countries")]
        public async Task<IActionResult> DeleteMovieCountry(Guid id, Guid countryId) {
            _logger.LogInformation("Deleting country {0} from movie {1}", countryId, id);
            var result = await _movieService.RemoveCountryAsync(id, countryId);
            if (!result.IsSuccess) {
                _logger.LogError(result.Errors);
                return BadRequest(new { result.Errors });
            }
            return NoContent();
        }

        #endregion Countries endpoints

        #region Artists endpoints

        // POST: api/movies/5/artists
        [HttpPost]
        [Route("{id}/[action]", Name = nameof(PostMovieArtist))]
        [ActionName("Artists")]
        public async Task<IActionResult> PostMovieArtist(Guid id, [FromBody] MovieArtistDto request) {
            _logger.LogInformation("Adding artist {0} to movie {1}", request, id);

            if (!ModelState.IsValid) {
                _logger.LogWarning("Validation failed: {0}", ModelState);
                return BadRequest();
            }

            var result = await _movieService.SaveArtistAsync(id, request);
            if (!result.IsSuccess) {
                _logger.LogError(result.Errors);
                return BadRequest(new { result.Errors });
            }

            var response = new LinkedResourceDto();
            EnrichMovieArtistResponse(id, request.ArtistId, response);
            return StatusCode((int)HttpStatusCode.Created, response);
        }

        // DELETE: api/movies/5/artists/3
        [HttpDelete]
        [Route("{id}/[action]/{artistId}", Name = nameof(DeleteMovieArtist))]
        [ActionName("Artists")]
        public async Task<IActionResult> DeleteMovieArtist(Guid id, Guid artistId, [FromBody] MovieArtistDto request) {
            _logger.LogInformation("Deleting artist {0} from movie {1}", request, id);

            if (artistId != request.ArtistId) {
                _logger.LogWarning("Validation failed: {0}", ModelState);
                return BadRequest();
            }

            var result = await _movieService.RemoveArtistAsync(id, request);
            if (!result.IsSuccess) {
                _logger.LogError(result.Errors);
                return BadRequest(new { result.Errors });
            }

            return NoContent();
        }

        #endregion Artists endpoints

        #region Response enrichers

        private void EnrichMovieDtoResponse(HyperMediaLinksDecorator<MovieDto> movieDto) {
            movieDto.Links.Add(new HyperMediaLink() {
                Action = HttpActionVerb.POST,
                Href = Url.Link(nameof(PostMovie), null),
                Rel = RelationType.self
            });

            movieDto.Links.Add(new HyperMediaLink() {
                Action = HttpActionVerb.POST,
                Href = Url.Link(nameof(PostMovieFromExternalSource), null),
                Rel = RelationType.self
            });

            movieDto.Links.Add(new HyperMediaLink() {
                Action = HttpActionVerb.POST,
                Href = Url.Link(nameof(PostMovieGenre), new { movieDto.Data.Id }),
                Rel = RelationType.self
            });

            movieDto.Links.Add(new HyperMediaLink() {
                Action = HttpActionVerb.POST,
                Href = Url.Link(nameof(PostMovieCountry), new { movieDto.Data.Id }),
                Rel = RelationType.self
            });

            movieDto.Links.Add(new HyperMediaLink() {
                Action = HttpActionVerb.POST,
                Href = Url.Link(nameof(PostMovieArtist), new { movieDto.Data.Id }),
                Rel = RelationType.self
            });
        }

        private void EnrichMovieGenreResponse(Guid movieId, Guid genreId, LinkedResourceDto linkedDto) {
            linkedDto.Links.Add(new HyperMediaLink() {
                Action = HttpActionVerb.POST,
                Href = Url.Link(nameof(PostMovieGenre), new { Id = movieId }),
                Rel = RelationType.self
            });
            linkedDto.Links.Add(new HyperMediaLink() {
                Action = HttpActionVerb.DELETE,
                Href = Url.Link(nameof(DeleteMovieGenre), new { Id = movieId, genreId }),
                Rel = RelationType.self
            });
        }

        private void EnrichMovieCountryResponse(Guid movieId, Guid countryId, LinkedResourceDto linkedDto) {
            linkedDto.Links.Add(new HyperMediaLink() {
                Action = HttpActionVerb.POST,
                Href = Url.Link(nameof(PostMovieCountry), new { Id = movieId }),
                Rel = RelationType.self
            });
            linkedDto.Links.Add(new HyperMediaLink() {
                Action = HttpActionVerb.DELETE,
                Href = Url.Link(nameof(DeleteMovieCountry), new { Id = movieId, countryId }),
                Rel = RelationType.self
            });
        }

        private void EnrichMovieArtistResponse(Guid movieId, Guid artistId, LinkedResourceDto linkedDto) {
            linkedDto.Links.Add(new HyperMediaLink() {
                Action = HttpActionVerb.POST,
                Href = Url.Link(nameof(PostMovieArtist), new { Id = movieId }),
                Rel = RelationType.self
            });
            linkedDto.Links.Add(new HyperMediaLink() {
                Action = HttpActionVerb.DELETE,
                Href = Url.Link(nameof(DeleteMovieArtist), new { Id = movieId, artistId }),
                Rel = RelationType.self
            });
        }

        #endregion Response enrichers
    }
}
