using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieLand.Api.Models;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Dtos.DataTables;
using MovieLand.BLL.Dtos.Genre;
using MovieLand.Data.ApplicationDbContext;
using MovieLand.Data.Models;

namespace MovieLand.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenresController(IGenreService genreService) {
            _genreService = genreService ?? throw new ArgumentNullException(nameof(genreService));
        }

        // GET: api/Genres
        [HttpGet]
        public async Task<IActionResult> Get(string search, [FromQuery]PaginationParameters pagination)
        {
            var param = new DataTablesParameters {
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
                    Value = search ?? "",
                    Regex = false
                }
            };
            var result = await _genreService.GetAsync(param);

            if (result.IsSuccess) {
                return Ok(new ArrayResult<GenreDto>(
                    result.Entity.Items
                ));
            }
            return StatusCode((int)HttpStatusCode.InternalServerError, new { result.Errors });
        }

        // GET: api/Genres/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Genre>> Get(Guid id)
        {
            var result = await _genreService.GetByIdAsync(id);
            if (result.Entity == null)
                return NotFound();
            return Ok(result.Entity);
        }

        // PUT: api/Genres/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, GenreDto genreDto)
        {
            if (id != genreDto.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest();

            var result = await _genreService.SaveAsync(genreDto);
            if(!result.IsSuccess)
                return BadRequest(new { result.Errors });
            return Ok(result.Entity);
        }

        // POST: api/Genres
        [HttpPost]
        public async Task<ActionResult<Genre>> Post(GenreDto genreDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await _genreService.SaveAsync(genreDto);
            if (!result.IsSuccess)
                return BadRequest(new { result.Errors });

            return StatusCode((int)HttpStatusCode.Created, result.Entity);
        }
    }
}
