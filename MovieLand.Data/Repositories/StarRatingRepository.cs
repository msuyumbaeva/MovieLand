using Microsoft.EntityFrameworkCore;
using MovieLand.Data.ApplicationDbContext;
using MovieLand.Data.Contracts.Repositories;
using MovieLand.Data.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Data.Repositories
{
    public class StarRatingRepository : Repository<StarRating>, IStarRatingRepository
    {
        public StarRatingRepository(AppDbContext context) : base(context) {
        }

        public async Task<double> GetAverageByMovieAsync(Guid movieId) {
            return await DbSet
                .Where(s => s.MovieId == movieId)
                .DefaultIfEmpty()
                .AverageAsync(s => s.Value * 1.0);
        }
    }
}
