using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Dtos.Genre;

namespace MovieLand.Controllers
{
    public class GenreController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenreController(IGenreService genreService) {
            _genreService = genreService ?? throw new ArgumentNullException(nameof(genreService));
        }

        // GET: Genre
        public async Task<IActionResult> Index() 
        {
            var genresResult = await _genreService.GetAllAsync();
            if (genresResult.IsSuccess)
                return View(genresResult.Entity);
            else
                return RedirectToAction("Error", "Home");
        }

        // GET: Genre/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Genre/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GenreDto genre)
        {
            if (ModelState.IsValid) {
                var genresResult = await _genreService.CreateAsync(genre);
                if (genresResult.IsSuccess)
                    return RedirectToAction(nameof(Index));
                else
                    AddErrors(genresResult.Errors);
            }
            return View(genre);
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
                var genresResult = await _genreService.EditAsync(genre);
                if (genresResult.IsSuccess)
                    return RedirectToAction(nameof(Index));
                else
                    AddErrors(genresResult.Errors);
            }
            return View(genre);
        }
    }
}