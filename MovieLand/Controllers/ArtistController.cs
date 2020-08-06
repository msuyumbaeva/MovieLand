using Microsoft.AspNetCore.Mvc;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Dtos.Artist;
using MovieLand.BLL.Dtos.DataTables;
using MovieLand.Models;
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
        public IActionResult Index() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoadTable([FromBody] DataTablesParameters param) {
            var result = await _artistService.GetAsync(param);
            if (result.IsSuccess) {
                return new JsonResult(new DataTablesResult<ArtistDto> {
                    Draw = param.Draw,
                    Data = result.Entity.Items,
                    RecordsFiltered = result.Entity.TotalSize,
                    RecordsTotal = result.Entity.TotalSize
                });
            }
            return new JsonResult(new { error = "Internal Server Error" });
        }

        // GET: Artist/Create
        public IActionResult Create() {
            var artistDto = new ArtistDto();
            return View("Edit", artistDto);
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
                var artistsResult = await _artistService.SaveAsync(artist);
                if (artistsResult.IsSuccess)
                    return RedirectToAction(nameof(Index));
                else
                    AddErrors(artistsResult.Errors);
            }
            return View(artist);
        }
    }
}
