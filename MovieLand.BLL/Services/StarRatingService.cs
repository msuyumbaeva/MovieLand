using AutoMapper;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Dtos.StarRating;
using MovieLand.Data.Builders;
using MovieLand.Data.Contracts.Repositories;
using MovieLand.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieLand.BLL.Services
{
    public class StarRatingService : BaseService, IStarRatingService
    {
        private readonly AppUserManager _userManager;

        public StarRatingService(IMapper mapper, IUnitOfWork unitOfWork, AppUserManager userManager) : base(mapper, unitOfWork) {
            _userManager = userManager;
        }

        // Create or Update star rating
        public async Task<OperationDetails<bool>> SaveAsync(StarRatingDto ratingDto) {
            try {
                // Find user
                var user = await _userManager.FindByIdAsync(ratingDto.User);
                if (user == null)
                    throw new Exception($"User with id {ratingDto.User} was not found");

                // Find movie
                var movie = await _unitOfWork.Movies.GetByIdAsync(ratingDto.MovieId);
                if (movie == null)
                    throw new Exception($"Movie with id {ratingDto.MovieId} was not found");

                // Map dto to entity
                var rating = _mapper.Map<StarRating>(ratingDto);
                rating.CreatedAt = DateTime.Now;

                // Filter by user id and movie id
                var queryBuilder = new EntityQueryBuilder<StarRating>();
                queryBuilder.SetFilter(s => s.UserId == user.Id && s.MovieId == ratingDto.MovieId);

                // Get rating
                var dbRating = await _unitOfWork.StarRatings.GetAsync(queryBuilder);
                // If exists
                if (dbRating.Count() > 0) {
                    // Update existing rating
                    _unitOfWork.StarRatings.Update(rating);
                }
                else {
                    // Create new rating
                    await _unitOfWork.StarRatings.AddAsync(rating);
                }
                // Save changes
                await _unitOfWork.CompleteAsync();

                // Get new average rating of movie
                var avg = await _unitOfWork.StarRatings.GetAverageByMovieAsync(ratingDto.MovieId);

                // Update average rating of movie
                movie.AvgRating = avg;
                _unitOfWork.Movies.Update(movie);

                // Save changes
                await _unitOfWork.CompleteAsync();
                return OperationDetails<bool>.Success(true);
            }
            catch (Exception ex) {
                return OperationDetails<bool>.Failure().AddError(ex.Message);
            }
        }

        // Get average rating of movie
        public async Task<OperationDetails<double>> GetAverageRatingOfMovieAsync(Guid movieId) {
            try {
                var movie = await _unitOfWork.Movies.GetByIdAsync(movieId);
                if (movie == null)
                    throw new Exception($"Movie with id {movieId} was not found");
                return OperationDetails<double>.Success(movie.AvgRating);
            }
            catch (Exception ex) {
                return OperationDetails<double>.Failure().AddError(ex.Message);
            }
        }
    }
}
