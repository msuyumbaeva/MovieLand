using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Dtos.Genre;

namespace MovieLand.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreApiController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenreApiController(IGenreService genreService) {
            _genreService = genreService ?? throw new ArgumentNullException(nameof(genreService));
        }

        // GET: api/GenreApi
        [HttpGet]
        public async Task<IEnumerable<GenreDto>> Get()
        {
            var genresResult = await _genreService.GetAllAsync();
            return genresResult.Entity;
        }
    }
}
