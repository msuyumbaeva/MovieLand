using MovieLand.BLL.Dtos.StarRating;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MovieLand.BLL.Contracts
{
    public interface IStarRatingService
    {
        // Get average rating of movie
        Task<OperationDetails<double>> GetAverageRatingOfMovieAsync(Guid movieId);
        // Create or Update star rating
        Task<OperationDetails<bool>> SaveAsync(StarRatingDto ratingDto);
    }
}
