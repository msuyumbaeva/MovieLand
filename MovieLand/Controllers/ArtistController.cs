using Microsoft.AspNetCore.Mvc;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Dtos.Artist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Controllers
{
    public class ArtistController : ControllerBase
    {
        private readonly IArtistService _artistService;

        public ArtistController(IArtistService artistService) {
            _artistService = artistService ?? throw new ArgumentNullException(nameof(artistService));
        }

        // GET: Artist
        public async Task<IActionResult> Index() {
            var artistsResult = await _artistService.GetAllAsync();
            if (artistsResult.IsSuccess)
                return View(artistsResult.Entity);
            else
                return RedirectToAction("Error", "Home");
        }

        // GET: Artist/Create
        public IActionResult Create() {
            return View();
        }

        // POST: Artist/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArtistDto artist) {
            if (ModelState.IsValid) {
                var artistsResult = await _artistService.CreateAsync(artist);
                if (artistsResult.IsSuccess)
                    return RedirectToAction(nameof(Index));
                else
                    AddErrors(artistsResult.Errors);
            }
            return View(artist);
        }

        // GET: Artist/Edit/5
        public async Task<IActionResult> Edit(Guid id) {
            var artistsResult = await _artistService.GetByIdAsync(id);
            if (artistsResult.IsSuccess)
                return View(artistsResult.Entity);
            else
                return NotFound();
        }

        // POST: Artist/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ArtistDto artist) {
            if (ModelState.IsValid) {
                var artistsResult = await _artistService.EditAsync(artist);
                if (artistsResult.IsSuccess)
                    return RedirectToAction(nameof(Index));
                else
                    AddErrors(artistsResult.Errors);
            }
            return View(artist);
        }
    }
}
