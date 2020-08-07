using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MovieLand.BLL.Configurations;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Dtos;
using MovieLand.BLL.Dtos.Artist;
using MovieLand.BLL.Dtos.Country;
using MovieLand.BLL.Dtos.DataTables;
using MovieLand.BLL.Dtos.Genre;
using MovieLand.BLL.Dtos.Movie;
using MovieLand.Data.ApplicationDbContext;
using MovieLand.Data.Builders;
using MovieLand.Data.Contracts.Repositories;
using MovieLand.Data.Enums;
using MovieLand.Data.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MovieLand.BLL.Services
{
    public class MovieService : BaseService,IMovieService
    {
        private readonly MoviePosterFileConfiguration _fileConfiguration;
        private readonly IFileClient _fileClient;

        public MovieService(IMapper mapper, IUnitOfWork unitOfWork, IOptions<MoviePosterFileConfiguration> fileConfiguration, IFileClient fileClient) :  base(mapper, unitOfWork){
            _fileConfiguration = fileConfiguration?.Value ?? throw new ArgumentNullException(nameof(fileConfiguration));
            _fileClient = fileClient ?? throw new ArgumentNullException(nameof(fileClient));
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
                movie = await _unitOfWork.Movies.AddAsync(movie);
                await _unitOfWork.CompleteAsync();

                // Map dto
                var movieDto = _mapper.Map<MovieDto>(movie);
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
                var movie = await _unitOfWork.Movies.GetByIdAsync(movieId);
                if (movie == null)
                    throw new Exception($"Movie with id {movieId} was not found.");

                // Get current genres of movie
                var currentMovieGenres = (await _unitOfWork.Movies.GetGenresByMovieAsync(movieId)).Select(g => g.Id);
                
                // Get genres that not already exist
                var movieGenresToAdd = genres.Except(currentMovieGenres);    
                // Add genres
                foreach(var genre in movieGenresToAdd) {
                    await _unitOfWork.Movies.AddToGenreAsync(new MovieGenre() { GenreId = genre, MovieId = movieId });
                }

                // Get genres that not exist in set list
                var movieGenresToDelete = currentMovieGenres.Except(genres);
                // Remove genres
                foreach (var genre in movieGenresToDelete) {
                    _unitOfWork.Movies.RemoveFromGenre(new MovieGenre() { GenreId = genre, MovieId = movieId });
                }

                // Save changes
                await _unitOfWork.CompleteAsync();

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
                var movie = await _unitOfWork.Movies.GetByIdAsync(movieId);
                if (movie == null)
                    throw new Exception($"Movie with id {movieId} was not found.");

                // Get current countries of movie
                var currentMovieCountries = (await _unitOfWork.Movies.GetCountriesByMovieAsync(movieId)).Select(c => c.Id);

                // Get countries that not already exist
                var movieCountriesToAdd = countries.Except(currentMovieCountries);
                // Add countries
                foreach (var country in movieCountriesToAdd) {
                    await _unitOfWork.Movies.AddToCountryAsync(new MovieCountry() { MovieId = movieId, CountryId = country });
                }

                // Get countries that not exist in set list
                var movieCountriesToDelete = currentMovieCountries.Except(countries);
                // Remove countries
                foreach (var country in movieCountriesToDelete) {
                    _unitOfWork.Movies.RemoveFromCountry(new MovieCountry() { MovieId = movieId, CountryId = country });
                }

                // Save changes
                await _unitOfWork.CompleteAsync();

                return OperationDetails<bool>.Success(true);
            }
            catch (Exception ex) {
                return OperationDetails<bool>.Failure().AddError(ex.Message);
            }
        }

        // Get movies list by page request
        public async Task<OperationDetails<DataTablesPagedResults<MovieListItemDto>>> GetAsync(DataTablesParameters table) {
            try {
                MovieListItemDto[] items = null;
                // Get total size
                var size = await _unitOfWork.Movies.CountAsync();

                /// Query building
                var queryBuilder = new EntityQueryBuilder<Movie>();

                // Filter
                if (!string.IsNullOrEmpty(table.Search.Value))
                    queryBuilder.SetFilter(m => m.Name.Contains(table.Search.Value) || m.OriginalName.Contains(table.Search.Value) || m.ReleaseYear.ToString() == table.Search.Value);

                // Order
                var order = table.Order[0];
                Expression<Func<Movie, object>> orderProperty = null;

                // Order property
                if (table.SortOrder == "Name")
                    orderProperty = m => m.Name;
                else if (table.SortOrder == "ReleaseYear")
                    orderProperty = m => m.ReleaseYear;

                // Order direction
                if (order.Dir == DTOrderDir.ASC)
                    queryBuilder.SetOrderBy(m => m.OrderBy(orderProperty));
                else
                    queryBuilder.SetOrderBy(m => m.OrderByDescending(orderProperty));

                // Limit
                queryBuilder.SetLimit(table.Length);

                // Offset
                queryBuilder.SetOffset((table.Start / table.Length) * table.Length);
                /// End Query building

                // Get genres
                var genres = await _unitOfWork.Movies.GetAsync(queryBuilder);
                // Map to dto
                items = _mapper.Map<MovieListItemDto[]>(genres);

                // Return result
                var result = new DataTablesPagedResults<MovieListItemDto> {
                    Items = items,
                    TotalSize = size
                };
                return OperationDetails<DataTablesPagedResults<MovieListItemDto>>.Success(result);
            }
            catch (Exception ex) {
                return OperationDetails<DataTablesPagedResults<MovieListItemDto>>.Failure().AddError(ex.Message);
            }
        }

        public async Task<OperationDetails<MovieDto>> GetById(Guid id) {
            try {
                var movie = await _unitOfWork.Movies.GetByIdAsync(id);
                if (movie == null)
                    throw new Exception($"Movie with Id {id} was not found");

                var movieDto = _mapper.Map<MovieDto>(movie);

                var genres = await _unitOfWork.Movies.GetGenresByMovieAsync(id);
                movieDto.Genres = _mapper.Map<List<GenreDto>>(genres);

                var countries = await _unitOfWork.Movies.GetCountriesByMovieAsync(id);
                movieDto.Countries = _mapper.Map<List<CountryDto>>(countries);

                var directors = await _unitOfWork.Movies.GetArtistsByMovieAndCareerAsync(id, CareerEnum.Director);
                movieDto.Directors = _mapper.Map<List<ArtistDto>>(directors);

                var actors = await _unitOfWork.Movies.GetArtistsByMovieAndCareerAsync(id, CareerEnum.Actor);
                movieDto.Actors = _mapper.Map<List<ArtistDto>>(actors);

                return OperationDetails<MovieDto>.Success(movieDto);
            }
            catch (Exception ex) {
                return OperationDetails<MovieDto>.Failure().AddError(ex.Message);
            }
        }

        public async Task<OperationDetails<bool>> AddArtist(Guid movieId, MovieArtistDto artist) {
            try {
                var movie = await _unitOfWork.Movies.GetByIdAsync(movieId);
                if (movie == null)
                    throw new Exception($"Movie with Id {movieId} was not found");

                var movieArtist = _mapper.Map<MovieArtist>(artist);
                movieArtist.MovieId = movieId;

                await _unitOfWork.Movies.AddToArtistAndCareerAsync(movieArtist);
                await _unitOfWork.CompleteAsync();
                return OperationDetails<bool>.Success(true);
            }
            catch (Exception ex) {
                return OperationDetails<bool>.Failure().AddError(ex.Message);
            }
        }

        public async Task<OperationDetails<bool>> RemoveArtist(Guid movieId, MovieArtistDto artist) {
            try {
                var movieArtist = await _unitOfWork.Movies.GetByMovieAndArtistAndCareer(movieId, artist.ArtistId, artist.CareerId);

                if(movieArtist == null)
                    throw new Exception($"Artist {artist.ArtistId} as {artist.CareerId.ToString()} in movie {movieId} was not found");

                _unitOfWork.Movies.RemoveFromArtistAndCareer(movieArtist);
                await _unitOfWork.CompleteAsync();
                return OperationDetails<bool>.Success(true);
            }
            catch (Exception ex) {
                return OperationDetails<bool>.Failure().AddError(ex.Message);
            }
        }

        #endregion Interface implementations
    }
}
