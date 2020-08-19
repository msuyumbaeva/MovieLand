using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieLand.Api.HyperMedia;
using MovieLand.Api.Models;
using MovieLand.Api.Models.Movie;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Dtos.Movie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MovieLand.Api.Controllers
{
    [Route("api/movies")]
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    public class AdminMoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public AdminMoviesController(IMovieService movieService) {
            _movieService = movieService ?? throw new ArgumentNullException(nameof(movieService));
        }

        #region Movies endpoints

        // POST: api/movies
        [HttpPost(Name = nameof(PostMovie))]
        [TypeFilter(typeof(HyperMediaFilter))]
        public async Task<IActionResult> PostMovie([FromForm] MovieCreateDto createDto) {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await _movieService.SaveAsync(createDto);
            if (!result.IsSuccess)
                return BadRequest(new { result.Errors });

            var response = new HyperMediaLinksDecorator<MovieDto>(result.Entity);
            EnrichMovieDtoResponse(response);
            return StatusCode((int)HttpStatusCode.Created, response);
        }

        #endregion Movies endpoints

        #region Genres endpoints

        // POST: api/movies/5/genres
        [HttpPost]
        [Route("{id}/[action]", Name = nameof(PostMovieGenre))]
        [ActionName("Genres")]
        public async Task<IActionResult> PostMovieGenre(Guid id, [FromBody] MovieGenreRequest request) {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await _movieService.AddGenreAsync(id, request.GenreId);
            if (!result.IsSuccess)
                return BadRequest(new { result.Errors });

            var response = new LinkedResourceDto();
            EnrichMovieGenreResponse(id, request.GenreId, response);
            return StatusCode((int)HttpStatusCode.Created, response);
        }

        // DELETE: api/movies/5/genres/3
        [HttpDelete]
        [Route("{id}/[action]/{genreId}", Name = nameof(DeleteMovieGenre))]
        [ActionName("Genres")]
        public async Task<IActionResult> DeleteMovieGenre(Guid id, Guid genreId) {
            var result = await _movieService.RemoveGenreAsync(id, genreId);
            if (!result.IsSuccess)
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return NoContent();
        }

        #endregion Genres endpoints

        #region Countries endpoints

        // POST: api/movies/5/countries
        [HttpPost]
        [Route("{id}/[action]", Name = nameof(PostMovieCountry))]
        [ActionName("Countries")]
        public async Task<IActionResult> PostMovieCountry(Guid id, [FromBody] MovieCountryRequest request) {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await _movieService.AddCountryAsync(id, request.CountryId);
            if (!result.IsSuccess)
                return BadRequest(new { result.Errors });

            var response = new LinkedResourceDto();
            EnrichMovieCountryResponse(id, request.CountryId, response);
            return StatusCode((int)HttpStatusCode.Created, response);
        }

        // DELETE: api/movies/5/countries/3
        [HttpDelete]
        [Route("{id}/[action]/{countryId}", Name = nameof(DeleteMovieCountry))]
        [ActionName("Countries")]
        public async Task<IActionResult> DeleteMovieCountry(Guid id, Guid countryId) {
            var result = await _movieService.RemoveCountryAsync(id, countryId);
            if (!result.IsSuccess)
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return NoContent();
        }

        #endregion Countries endpoints

        #region Artists endpoints

        // POST: api/movies/5/artists
        [HttpPost]
        [Route("{id}/[action]", Name = nameof(PostMovieArtist))]
        [ActionName("Artists")]
        public async Task<IActionResult> PostMovieArtist(Guid id, [FromBody] MovieArtistDto request) {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await _movieService.SaveArtistAsync(id, request);
            if (!result.IsSuccess)
                return BadRequest(new { result.Errors });

            var response = new LinkedResourceDto();
            EnrichMovieArtistResponse(id, request.ArtistId, response);
            return StatusCode((int)HttpStatusCode.Created, response);
        }

        // DELETE: api/movies/5/artists/3
        [HttpDelete]
        [Route("{id}/[action]/{artistId}", Name = nameof(DeleteMovieArtist))]
        [ActionName("Artists")]
        public async Task<IActionResult> DeleteMovieArtist(Guid id, Guid artistId, [FromBody] MovieArtistDto request) {
            if (artistId != request.ArtistId)
                return BadRequest();

            var result = await _movieService.RemoveArtistAsync(id, request);
            if (!result.IsSuccess)
                return BadRequest(new { result.Errors });

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
