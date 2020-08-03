using Microsoft.AspNetCore.Mvc;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Dtos.Artist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistApiController : ControllerBase
    {
        private readonly IArtistService _artistService;

        public ArtistApiController(IArtistService artistService) {
            _artistService = artistService ?? throw new ArgumentNullException(nameof(artistService));
        }

        // GET: api/GenreApi
        [HttpGet]
        public async Task<IEnumerable<ArtistDto>> Get() {
            var artistsResult = await _artistService.GetAllAsync();
            return artistsResult.Entity;
        }
    }
}
