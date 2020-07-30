using Microsoft.AspNetCore.Mvc;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Dtos.Movie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Controllers
{
    public class MovieController: ControllerBase
    {
        private readonly IMovieService _movieService;

        public MovieController(IMovieService movieService) {
            _movieService = movieService ?? throw new ArgumentNullException(nameof(movieService));
        }

        public IActionResult Index() {
            return View();
        }

        // GET: Movie/Create
        public IActionResult Create() {
            return View();
        }

        // POST: Movie/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieCreateDto movie) {
            if (ModelState.IsValid) {
                var countriesResult = await _movieService.CreateAsync(movie);
                if (countriesResult.IsSuccess)
                    return RedirectToAction(nameof(Index));
                else
                    AddErrors(countriesResult.Errors);
            }
            return View(movie);
        }
    }
}
