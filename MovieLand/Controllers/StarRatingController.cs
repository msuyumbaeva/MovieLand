using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Dtos.StarRating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
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
        [Authorize(Roles = "USER")]
        public async Task<IActionResult> Create([FromBody] StarRatingDto ratingDto) {
            if (!ModelState.IsValid)
                return BadRequest();

            var userId = User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return StatusCode((int)HttpStatusCode.Forbidden);

            ratingDto.User = userId;
            var result = await _starRatingService.SaveAsync(ratingDto);
            if (!result.IsSuccess)
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(result);
        }
    }
}
