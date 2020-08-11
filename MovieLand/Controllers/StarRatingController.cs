using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Dtos.StarRating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MovieLand.Controllers
{
    public class StarRatingController : ControllerBase
    {
        private readonly IStarRatingService _starRatingService;

        public StarRatingController(IStarRatingService starRatingService) {
            _starRatingService = starRatingService ?? throw new ArgumentNullException(nameof(starRatingService));
        }

        [HttpGet]
        public async Task<IActionResult> GetAverageRatingOfMovie(Guid id) {
            var result = await _starRatingService.GetAverageRatingOfMovieAsync(id);
            if (!result.IsSuccess)
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StarRatingDto ratingDto) {
            if (!User.Identity.IsAuthenticated) 
                return new JsonResult(new { error = "Not authenticated" });

            if (!ModelState.IsValid)
                return BadRequest();

            ratingDto.UserName = User.Identity.Name;
            var result = await _starRatingService.SaveAsync(ratingDto);
            if (!result.IsSuccess)
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(result);
        }

        public async Task<IActionResult> GetByUserAndMovie(Guid movieId) {
            if (!User.Identity.IsAuthenticated)
                return new JsonResult(new { error = "Not authenticated" });

            if(!User.IsInRole("USER"))
                return new JsonResult(new { error = "Not allowed" });

            var userName = User.Identity.Name;
            var result = await _starRatingService.GetByUserAndMovieAsync(userName, movieId);
            if (!result.IsSuccess)
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(result);
        }
    }
}
