using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Dtos.Country;
using MovieLand.Data.Models;

namespace MovieLand.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryApiController : ControllerBase
    {
        private readonly ICountryService _countryService;

        public CountryApiController(ICountryService countryService) {
            _countryService = countryService ?? throw new ArgumentNullException(nameof(countryService));
        }

        // GET: api/GenreApi
        [HttpGet]
        public async Task<IEnumerable<CountryDto>> Get() {
            var genresResult = await _countryService.GetAllAsync();
            return genresResult.Entity;
        }
    }
}