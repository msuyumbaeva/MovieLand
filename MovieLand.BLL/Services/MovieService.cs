using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MovieLand.BLL.Configurations;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Dtos.Movie;
using MovieLand.Data.ApplicationDbContext;
using MovieLand.Data.Models;
using System;
using System.Collections.Generic;
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

        // Create movie
        public async Task<OperationDetails<Movie>> CreateAsync(MovieCreateDto movieDto) {
            try {
                // Save poster
                var posterFileName = await SavePoster(movieDto.Poster);

                var movie = _mapper.Map<Movie>(movieDto);
                movie.Poster = posterFileName;
                var movieEntry = await _context.Movies.AddAsync(movie);
                await _context.SaveChangesAsync();

                return OperationDetails<Movie>.Success(movieEntry.Entity);
            }
            catch (Exception ex) {
                return OperationDetails<Movie>.Failure().AddError(ex.Message);
            }
        }

        // Save poster
        // Returns new file name
        public async Task<string> SavePoster(IFormFile formFile) {
            var fileName = string.Concat(DateTime.Now.ToBinary(),formFile.FileName);

            using (var fileStream = formFile.OpenReadStream()) {
                await _fileClient.SaveFileAsync(_fileConfiguration.Directory, fileName, fileStream);
            }

            return fileName;
        }
    }
}
