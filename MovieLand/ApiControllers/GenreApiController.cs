using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieLand.BLL.Contracts;
using MovieLand.Data.Models;

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
        public async Task<IEnumerable<Genre>> Get()
        {
            var genresResult = await _genreService.GetAllAsync();
            return genresResult.Entity;
        }
    }
}
