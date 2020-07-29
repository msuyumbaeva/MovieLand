using Microsoft.AspNetCore.Mvc;
using MovieLand.BLL.Contracts;
using MovieLand.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Controllers
{
    public class CountryController : ControllerBase
    {
        private readonly ICountryService _countryService;

        public CountryController(ICountryService countryService) {
            _countryService = countryService ?? throw new ArgumentNullException(nameof(countryService));
        }

        // GET: Country
        public async Task<IActionResult> Index() {
            var countriesResult = await _countryService.GetAllAsync();
            if (countriesResult.IsSuccess)
                return View(countriesResult.Entity);
            else
                return RedirectToAction("Error", "Home");
        }

        // GET: Country/Create
        public IActionResult Create() {
            return View();
        }

        // POST: Country/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Country country) {
            if (ModelState.IsValid) {
                var countriesResult = await _countryService.CreateAsync(country);
                if (countriesResult.IsSuccess)
                    return RedirectToAction(nameof(Index));
                else
                    AddErrors(countriesResult.Errors);
            }
            return View(country);
        }

        // GET: Country/Edit/5
        public async Task<IActionResult> Edit(Guid id) {
            var countriesResult = await _countryService.GetByIdAsync(id);
            if (countriesResult.IsSuccess)
                return View(countriesResult.Entity);
            else
                return NotFound();
        }

        // POST: Country/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Country country) {
            if (ModelState.IsValid) {
                var countriesResult = await _countryService.EditAsync(country);
                if (countriesResult.IsSuccess)
                    return RedirectToAction(nameof(Index));
                else
                    AddErrors(countriesResult.Errors);
            }
            return View(country);
        }
    }
}
