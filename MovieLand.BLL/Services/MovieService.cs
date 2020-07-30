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

        // Create movie
        public async Task<OperationDetails<Movie>> CreateAsync(MovieCreateDto movieDto) {
            try {
                // Save poster
                var posterFileName = await SavePoster(movieDto.Poster);

                // Save movie to db
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

        // Saves poster
        // Returns new file name
        public async Task<string> SavePoster(IFormFile formFile) {
            // Generate unique file name
            var fileName = string.Concat(DateTime.Now.ToBinary(),formFile.FileName);

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
    }
}
