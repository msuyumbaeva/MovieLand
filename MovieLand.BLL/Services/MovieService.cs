using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MovieLand.BLL.Configurations;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Dtos.Movie;
using MovieLand.Data.ApplicationDbContext;
using MovieLand.Data.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieLand.BLL.Services
{
    public class MovieService : IMovieService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly MoviePosterFileConfiguration _fileConfiguration;
        private readonly IFileClient _fileClient;

        public MovieService(AppDbContext context, IMapper mapper, IFileClient fileClient, IOptions<MoviePosterFileConfiguration> options) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _fileClient = fileClient ?? throw new ArgumentNullException(nameof(fileClient));
            _fileConfiguration = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        #region Private methods
        // Saves poster
        // Returns new file name
        private async Task<string> SavePoster(IFormFile formFile) {
            // Generate unique file name
            var fileName = string.Concat(DateTime.Now.ToBinary(), formFile.FileName);

            // Resize image and save to stream
            var outputStream = new MemoryStream();
            using (var image = Image.Load(formFile.OpenReadStream())) {
                image.Mutate(x => x.Resize(_fileConfiguration.Width, _fileConfiguration.Height));
                image.SaveAsJpeg(outputStream);
            }

            // Activate stream
            outputStream.Seek(0, SeekOrigin.Begin);

            // Make file client save image
            using (outputStream) {
                await _fileClient.SaveFileAsync(_fileConfiguration.Directory, fileName, outputStream);
            }

            // Return new file name
            return fileName;
        }
        #endregion Private methods

        #region Interface implementations
        // Create movie
        public async Task<OperationDetails<MovieDto>> CreateAsync(MovieCreateDto movieCreateDto) {
            try {
                // Save poster
                var posterFileName = await SavePoster(movieCreateDto.Poster);

                // Save movie to db
                var movie = _mapper.Map<Movie>(movieCreateDto);
                movie.Poster = posterFileName;
                var movieEntry = await _context.Movies.AddAsync(movie);
                await _context.SaveChangesAsync();

                // Map dto
                var movieDto = _mapper.Map<MovieDto>(movieEntry.Entity);
                return OperationDetails<MovieDto>.Success(movieDto);
            }
            catch (Exception ex) {
                return OperationDetails<MovieDto>.Failure().AddError(ex.Message);
            }
        }

        // Add genres to movie
        public async Task<OperationDetails<bool>> SetGenres(Guid movieId, ICollection<Guid> genres) {
            try {
                // Find movie by id
                var movie = await _context.Movies.FindAsync(movieId);
                if (movie == null)
                    throw new Exception($"Movie with id {movieId} was not found.");

                // Get current genres of movie
                var currentMovieGenres = await _context.MovieGenres.Where(m => m.MovieId == movieId).Select(m => m.GenreId).ToListAsync();
                
                // Get genres that not already exist
                var movieGenresToAdd = genres.Except(currentMovieGenres);    
                // Add genres
                foreach(var genre in movieGenresToAdd) {
                    await _context.MovieGenres.AddAsync(new MovieGenre() { MovieId = movieId, GenreId = genre });
                }

                // Get genres that not exist in set list
                var movieGenresToDelete = currentMovieGenres.Except(genres);
                // Remove genres
                foreach (var genre in movieGenresToDelete) {
                    var entitiesToDelete = await _context.MovieGenres.Where(m=>m.MovieId == movieId && m.GenreId == genre).FirstOrDefaultAsync();
                    _context.MovieGenres.Remove(entitiesToDelete);
                }

                // Save changes
                await _context.SaveChangesAsync();

                return OperationDetails<bool>.Success(true);
            }
            catch (Exception ex) {
                return OperationDetails<bool>.Failure().AddError(ex.Message);
            }
        }

        // Add countries to movie
        public async Task<OperationDetails<bool>> SetCountries(Guid movieId, ICollection<Guid> countries) {
            try {
                // Find movie by id
                var movie = await _context.Movies.FindAsync(movieId);
                if (movie == null)
                    throw new Exception($"Movie with id {movieId} was not found.");

                // Get current countries of movie
                var currentMovieCountries = await _context.MovieContries.Where(m => m.MovieId == movieId).Select(m => m.CountryId).ToListAsync();

                // Get countries that not already exist
                var movieCountriesToAdd = countries.Except(currentMovieCountries);
                // Add countries
                foreach (var country in movieCountriesToAdd) {
                    await _context.MovieContries.AddAsync(new MovieCountry() { MovieId = movieId, CountryId = country });
                }

                // Get countries that not exist in set list
                var movieCountriesToDelete = currentMovieCountries.Except(countries);
                // Remove countries
                foreach (var country in movieCountriesToDelete) {
                    var entitiesToDelete = await _context.MovieContries.Where(m => m.MovieId == movieId && m.CountryId == country).FirstOrDefaultAsync();
                    _context.MovieContries.Remove(entitiesToDelete);
                }

                // Save changes
                await _context.SaveChangesAsync();

                return OperationDetails<bool>.Success(true);
            }
            catch (Exception ex) {
                return OperationDetails<bool>.Failure().AddError(ex.Message);
            }
        }
        #endregion Interface implementations
    }
}
