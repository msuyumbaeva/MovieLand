using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Dtos.Country;
using MovieLand.BLL.Dtos.DataTables;
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
        public async Task<DataTablesPagedResults<CountryDto>> Get(string search) {
            var parameters = new DataTablesParameters();
            parameters.Search = new DTSearch() { Value = search, Regex = false };
            parameters.Order = new DTOrder[1] {
                new DTOrder() { Column = 1, Dir = DTOrderDir.ASC }
            };
            parameters.Columns = new DTColumn[2] {
                new DTColumn() { Data = "Id", Name = "Id", Orderable = false, Searchable = false },
                new DTColumn() { Data = "Name", Name = "Name", Orderable = true, Searchable = true }
            };
            parameters.Start = 0;
            var genresResult = await _countryService.GetAsync(parameters);
            return genresResult.Entity;
        }
    }
}