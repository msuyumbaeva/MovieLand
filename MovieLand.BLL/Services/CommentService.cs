using AutoMapper;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Dtos.Comment;
using MovieLand.BLL.Dtos.DataTables;
using MovieLand.Data.Builders;
using MovieLand.Data.Contracts.Repositories;
using MovieLand.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MovieLand.BLL.Services
{
    public class CommentService : BaseService, ICommentService
    {
        private readonly AppUserManager _userManager;

        public CommentService(IMapper mapper, IUnitOfWork unitOfWork, AppUserManager userManager) : base(mapper, unitOfWork) {
            _userManager = userManager;
        }

        // Create comment
        public async Task<OperationDetails<bool>> CreateAsync(CommentDto commentDto) {
            try {
                // Find user
                var user = await _userManager.FindByNameAsync(commentDto.UserName);
                if (user == null)
                    throw new Exception($"User {commentDto.UserName} was not found");

                // Map dto to entity
                var comment = _mapper.Map<Comment>(commentDto);
                comment.CreatedAt = DateTime.Now;
                comment.UserId = user.Id;

                // Create comment
                await _unitOfWork.Comments.AddAsync(comment);
                await _unitOfWork.CompleteAsync();
                return OperationDetails<bool>.Success(true);
            }
            catch (Exception ex) {
                return OperationDetails<bool>.Failure().AddError(ex.Message);
            }
        }

        // Get comments of movie
        public async Task<OperationDetails<DataTablesPagedResults<CommentDto>>> GetByMovieIdAsync(Guid movieId, DataTablesParameters table) {
            try {
                CommentDto[] items = null;
                // Get total size
                var size = await _unitOfWork.Comments.CountAsync();

                /// Query building
                var queryBuilder = new EntityQueryBuilder<Comment>();

                // Filter
                queryBuilder.SetFilter(m => m.MovieId == movieId);

                // Order
                queryBuilder.SetOrderBy(m => m.OrderBy(c => c.CreatedAt));

                // Limit
                queryBuilder.SetLimit(table.Length);

                if (table.Length > 0) {
                    // Offset
                    queryBuilder.SetOffset((table.Start / table.Length) * table.Length);
                }

                // Includes
                queryBuilder.AddInclude(m => m.User);
                /// End Query building

                // Get comments
                var comments = await _unitOfWork.Comments.GetAsync(queryBuilder);
                // Map to dto
                items = _mapper.Map<CommentDto[]>(comments);

                // Return result
                var result = new DataTablesPagedResults<CommentDto> {
                    Items = items,
                    TotalSize = size
                };
                return OperationDetails<DataTablesPagedResults<CommentDto>>.Success(result);
            }
            catch (Exception ex) {
                return OperationDetails<DataTablesPagedResults<CommentDto>>.Failure().AddError(ex.Message);
            }
        }
    }
}
