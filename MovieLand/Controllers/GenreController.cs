using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Dtos.DataTables;
using MovieLand.BLL.Dtos.Genre;
using MovieLand.Models;

namespace MovieLand.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class GenreController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenreController(IGenreService genreService) {
            _genreService = genreService ?? throw new ArgumentNullException(nameof(genreService));
        }

        // GET: Genre
        public IActionResult Index() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoadTable([FromBody] DataTablesParameters param) {
            var result = await _genreService.GetAsync(param);
            if (result.IsSuccess) {
                return new JsonResult(new DataTablesResult<GenreDto> {
                    Draw = param.Draw,
                    Data = result.Entity.Items,
                    RecordsFiltered = result.Entity.TotalSize,
                    RecordsTotal = result.Entity.TotalSize
                });
            }
            return new JsonResult(new { error = "Internal Server Error" });
        }

        // GET: Genre/Create
        public IActionResult Create() {
            var genreDto = new GenreDto();
            return View("Edit", genreDto);
        }

        // GET: Genre/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            var genresResult = await _genreService.GetByIdAsync(id);
            if (genresResult.IsSuccess)
                return View(genresResult.Entity);
            else
                return NotFound();
        }

        // POST: Genre/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(GenreDto genre)
        {
            if (ModelState.IsValid) {
                var genresResult = await _genreService.SaveAsync(genre);
                if (genresResult.IsSuccess)
                    return RedirectToAction(nameof(Index));
                else
                    AddErrors(genresResult.Errors);
            }
            return View(genre);
        }
    }
}