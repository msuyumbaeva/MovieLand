using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Dtos;
using MovieLand.BLL.Dtos.Artist;
using MovieLand.BLL.Dtos.Country;
using MovieLand.BLL.Dtos.DataTables;
using MovieLand.BLL.Dtos.Genre;
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

        // GET: Movie/Index
        public IActionResult Index() {
            return View();
        }

        public async Task<IActionResult> LoadTable([FromBody] DataTablesParameters param) {
            var result = await _movieService.GetAsync(param);
            if (result.IsSuccess) {
                return new JsonResult(new DataTablesResult<MovieListItemDto> {
                    Draw = param.Draw,
                    Data = result.Entity.Items,
                    RecordsFiltered = result.Entity.TotalSize,
                    RecordsTotal = result.Entity.TotalSize
                });
            }
            return new JsonResult(new { error = "Internal Server Error" });
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

        public async Task<IActionResult> Edit(Guid id) {
            var result = await _movieService.GetById(id);
            if (!result.IsSuccess)
                return NotFound();

            var dto = _mapper.Map<MovieCreateViewModel>(result.Entity);
            dto.Artists.Add(new NamedArray<Guid>() { Name = CareerEnum.Director.ToString() });
            dto.Artists.Add(new NamedArray<Guid>() { Name = CareerEnum.Actor.ToString() });
            return View(dto);
        }

        public async Task<IEnumerable<GenreDto>> GetGenres(Guid id) {
            var result = await _movieService.GetGenresOfMovie(id);
            return result.Entity;
        }

        public async Task<IEnumerable<CountryDto>> GetCountries(Guid id) {
            var result = await _movieService.GetCountriesOfMovie(id);
            return result.Entity;
        }

        public async Task<IEnumerable<ArtistDto>> GetArtists(Guid id, CareerEnum career) {
            var result = await _movieService.GetArtistsByCareerOfMovie(id, career);
            return result.Entity;
        }
    }
}
