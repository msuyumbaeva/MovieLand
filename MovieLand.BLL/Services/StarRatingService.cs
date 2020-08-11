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
                var user = await _userManager.FindByNameAsync(ratingDto.UserName);
                if (user == null)
                    throw new Exception($"User {ratingDto.UserName} was not found");

                // Map dto to entity
                var rating = _mapper.Map<StarRating>(ratingDto);
                rating.UserId = user.Id;
                rating.CreatedAt = DateTime.Now;

                // Filter by user id and movie id
                var queryBuilder = new EntityQueryBuilder<StarRating>();
                queryBuilder.SetFilter(s => s.UserId == user.Id && s.MovieId == ratingDto.MovieId);

                // Get rating
                var dbRating = await _unitOfWork.StarRatings.GetAsync(queryBuilder);
                if (dbRating.Count() > 0) {
                    // Update existing rating
                    _unitOfWork.StarRatings.Update(rating);
                }
                else {
                    // Create new rating
                    await _unitOfWork.StarRatings.AddAsync(rating);
                }
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
                var avg = await _unitOfWork.StarRatings.GetAverageByMovieAsync(movieId);
                return OperationDetails<double>.Success(avg);
            }
            catch(Exception ex) {
                return OperationDetails<double>.Failure().AddError(ex.Message);
            }
        }

        // Get rating of movie by user
        public async Task<OperationDetails<StarRatingDto>> GetByUserAndMovieAsync(string userName, Guid movieId) {
            try {
                // Find user
                var user = await _userManager.FindByNameAsync(userName);
                if (user == null)
                    throw new Exception($"User {userName} was not found");

                // Filter by user id and movie id
                var queryBuilder = new EntityQueryBuilder<StarRating>();
                queryBuilder.SetFilter(s=>s.UserId == user.Id && s.MovieId == movieId);

                // Get rating
                var rating = await _unitOfWork.StarRatings.GetAsync(queryBuilder);
                // Map to dto
                var dto = _mapper.Map<StarRatingDto>(rating.FirstOrDefault());
                return OperationDetails<StarRatingDto>.Success(dto);
            }
            catch (Exception ex) {
                return OperationDetails<StarRatingDto>.Failure().AddError(ex.Message);
            }
        }
    }
}
