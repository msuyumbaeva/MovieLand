using Microsoft.EntityFrameworkCore;
using MovieLand.Data.ApplicationDbContext;
using MovieLand.Data.Contracts.Repositories;
using MovieLand.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieLand.Data.Repositories
{
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        public CommentRepository(AppDbContext context) : base(context) {
        }

        public async Task<int> CountByMovieAsync(Guid movieId) {
            return await DbSet
                .Where(r => r.MovieId == movieId)
                .CountAsync();
        }
    }
}
