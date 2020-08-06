using Microsoft.AspNetCore.Mvc;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Dtos.Country;
using MovieLand.BLL.Dtos.DataTables;
using MovieLand.Models;
using System;
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
        public IActionResult Index() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoadTable([FromBody] DataTablesParameters param) {
            var result = await _countryService.GetAsync(param);
            if (result.IsSuccess) {
                return new JsonResult(new DataTablesResult<CountryDto> {
                    Draw = param.Draw,
                    Data = result.Entity.Items,
                    RecordsFiltered = result.Entity.TotalSize,
                    RecordsTotal = result.Entity.TotalSize
                });
            }
            return new JsonResult(new { error = "Internal Server Error" });
        }

        // GET: Country/Create
        public IActionResult Create() {
            var countryDto = new CountryDto();
            return View("Edit", countryDto);
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
        public async Task<IActionResult> Edit(CountryDto country) {
            if (ModelState.IsValid) {
                var countriesResult = await _countryService.SaveAsync(country);
                if (countriesResult.IsSuccess)
                    return RedirectToAction(nameof(Index));
                else
                    AddErrors(countriesResult.Errors);
            }
            return View(country);
        }
    }
}
