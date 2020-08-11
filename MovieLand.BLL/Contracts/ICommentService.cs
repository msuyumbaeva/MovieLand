using MovieLand.BLL.Dtos.Comment;
using MovieLand.BLL.Dtos.DataTables;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MovieLand.BLL.Contracts
{
    public interface ICommentService
    {
        // Create comment
        Task<OperationDetails<bool>> CreateAsync(CommentDto commentDto);
        // Get comments of movie
        Task<OperationDetails<DataTablesPagedResults<CommentDto>>> GetByMovieIdAsync(Guid movieId, DataTablesParameters table);
    }
}
