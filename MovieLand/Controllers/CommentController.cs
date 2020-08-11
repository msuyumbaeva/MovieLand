using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Dtos.Comment;
using MovieLand.BLL.Dtos.DataTables;
using MovieLand.Models;
using MovieLand.ViewModels.Comment;

namespace MovieLand.Controllers
{
    public class CommentController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICommentService _commentService;

        public CommentController(IMapper mapper, ICommentService commentService) {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _commentService = commentService ?? throw new ArgumentNullException(nameof(commentService));
        }

        public async Task<IActionResult> LoadTable(Guid id, [FromBody] DataTablesParameters param) {
            var result = await _commentService.GetByMovieIdAsync(id, param);
            if (result.IsSuccess) {
                return new JsonResult(new DataTablesResult<CommentDto> {
                    Draw = param.Draw,
                    Data = result.Entity.Items,
                    RecordsFiltered = result.Entity.TotalSize,
                    RecordsTotal = result.Entity.TotalSize
                });
            }
            return new JsonResult(new { error = "Internal Server Error" });
        }

        [Authorize(Roles = "USER")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CommentCreateViewModel viewModel) {
            if (ModelState.IsValid) {
                var commentDto = _mapper.Map<CommentDto>(viewModel);
                commentDto.UserName = User.Identity.Name;
                var result = await _commentService.CreateAsync(commentDto);
                if (!result.IsSuccess) {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                return Json(result);
            }
            return BadRequest();
        }

    }
}