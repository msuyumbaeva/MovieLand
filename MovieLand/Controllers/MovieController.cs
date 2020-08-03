using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Dtos;
using MovieLand.BLL.Dtos.Movie;
using MovieLand.Data.Enums;
using MovieLand.Models;
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

        // GET: Movie/Index?page=1
        public async Task<IActionResult> Index(int? page) {
            var pageSize = 10;
            var moviesResult = await _movieService.GetAllAsync(new Page(page ?? 1, pageSize));

            var viewModel = new PaginatedList<MovieListItemDto>(moviesResult.Entity.Items, moviesResult.Entity.TotalAmount, moviesResult.Entity.Page.Number, pageSize);
            return View(viewModel);
        }

        public async Task<IActionResult> Details(Guid id) {
            var movieResult = await _movieService.GetById(id);
            if (movieResult.Entity == null)
                return NotFound();
            else
                return View(movieResult.Entity);
        }

// GET: Movie/Create
public IActionResult Create() {
            return View();
        }

        // POST: Movie/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieCreateViewModel movieViewModel) {
            // Check if model is valid
            if (ModelState.IsValid) {
                // Map to dto
                var movieCreateDto = _mapper.Map<MovieCreateDto>(movieViewModel);
                // Create movie
                var movieResult = await _movieService.CreateAsync(movieCreateDto);
                // If created successfully
                if (movieResult.IsSuccess) {
                    var movieId = movieResult.Entity.Id;
                    // Set genres of movie
                    await _movieService.SetGenres(movieId, movieViewModel.Genres);

                    // Set countries of movie
                    await _movieService.SetCountries(movieId, movieViewModel.Countries);

                    // Add movie artists
                    foreach (var artists in movieViewModel.Artists) {
                        var career = (CareerEnum)Enum.Parse(typeof(CareerEnum), artists.Name, true);
                        for (int j = 0; j < artists.Items.Length; j++) {
                            var artistDto = new MovieArtistDto(artists.Items[j], career, (byte)(j + 1));
                            await _movieService.AddArtist(movieId, artistDto);
                        }
                    }

                    // Redirect to index action
                    return RedirectToAction(nameof(Index));
                }
                else
                    AddErrors(movieResult.Errors);
            }
            return View(movieViewModel);
        }
    }
}
