using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Dtos.Movie;
using MovieLand.ViewModels.Movie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Controllers
{
    public class MovieController: ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly IMapper _mapper;

        public MovieController(IMovieService movieService, IMapper mapper) {
            _movieService = movieService ?? throw new ArgumentNullException(nameof(movieService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
        public async Task<IActionResult> Create(MovieCreateViewModel movieViewModel) {
            if (ModelState.IsValid) {
                var movieCreateDto = _mapper.Map<MovieCreateDto>(movieViewModel);
                var movieResult = await _movieService.CreateAsync(movieCreateDto);
                if (movieResult.IsSuccess) {
                    await _movieService.SetGenres(movieResult.Entity.Id, movieViewModel.Genres);
                    await _movieService.SetCountries(movieResult.Entity.Id, movieViewModel.Countries);
                    return RedirectToAction(nameof(Index));
                }
                else
                    AddErrors(movieResult.Errors);
            }
            return View(movieViewModel);
        }
    }
}
