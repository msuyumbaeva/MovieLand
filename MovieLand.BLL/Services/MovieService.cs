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
using MovieLand.BLL.Extensions;
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
        private readonly static string NO_MOVIE_POSTER_FILE_NAME = "no-movie-poster.svg";

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
        // Create or Edit movie
        public async Task<OperationDetails<MovieDto>> SaveAsync(MovieCreateDto movieCreateDto) {
            try {
                var posterFileName = NO_MOVIE_POSTER_FILE_NAME;
                if (movieCreateDto.Poster != null) {
                    // Save poster
                    posterFileName = await SavePoster(movieCreateDto.Poster);
                }

                Movie movie = null;
                // Check if id is empty
                if (movieCreateDto.Id == Guid.Empty) {
                    // Create new movie
                    movie = _mapper.Map<Movie>(movieCreateDto);
                    movie.Poster = posterFileName;
                    movie = await _unitOfWork.Movies.AddAsync(movie);
                }
                else {
                    // Update existing movie
                    movie = await _unitOfWork.Movies.GetByIdAsync(movieCreateDto.Id);
                    if (movie == null)
                        throw new Exception($"Movie with Id {movieCreateDto.Id} was not found");

                    var moviePoster = movie.Poster;
                    movie = _mapper.Map(movieCreateDto, movie);

                    movie.Poster = movieCreateDto.Poster == null ? moviePoster : posterFileName;
                    _unitOfWork.Movies.Update(movie);
                }

                await _unitOfWork.CompleteAsync();

                // Map dto
                var movieDto = _mapper.Map<MovieDto>(movie);
                return OperationDetails<MovieDto>.Success(movieDto);
            }
            catch (Exception ex) {
                return OperationDetails<MovieDto>.Failure().AddError(ex.Message);
            }
        }

        // Get movies list by page request
        public async Task<OperationDetails<DataTablesPagedResults<MovieListItemDto>>> GetAsync(MovieDataTablesParameters table) {
            try {
                MovieListItemDto[] items = null;
                // Get total size
                var size = await _unitOfWork.Movies.CountAsync();

                /// Query building
                var queryBuilder = new EntityQueryBuilder<Movie>();

                // Filter
                Expression<Func<Movie, bool>> filter = m => true;
                // Genre
                if (table.Genre.HasValue)
                    filter = filter.CombineWithAndAlso(m => m.MovieGenres.Count(mg => mg.GenreId == table.Genre.Value) > 0);
                // Country
                if (table.Country.HasValue)
                    filter = filter.CombineWithAndAlso(m => m.MovieCountries.Count(mg => mg.CountryId == table.Country.Value) > 0);
                // Artist
                if (table.Artist.HasValue)
                    filter = filter.CombineWithAndAlso(m => m.MovieArtists.Count(mg => mg.ArtistId == table.Artist.Value) > 0);
                // Search
                if (!string.IsNullOrEmpty(table.Search.Value))
                    filter = filter.CombineWithAndAlso(m => m.Name.Contains(table.Search.Value) || m.OriginalName.Contains(table.Search.Value) || m.ReleaseYear.ToString() == table.Search.Value);

                queryBuilder.SetFilter(filter);
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

                if (table.Length > 0) {
                    // Offset
                    queryBuilder.SetOffset((table.Start / table.Length) * table.Length);
                }
                /// End Query building

                // Get movies
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

        // Get movie by Id
        public async Task<OperationDetails<MovieDto>> GetByIdAsync(Guid id) {
            try {
                var movie = await _unitOfWork.Movies.GetFullByIdAsync(id);
                if (movie == null)
                    throw new Exception($"Movie with Id {id} was not found");

                var movieDto = _mapper.Map<MovieDto>(movie);
                return OperationDetails<MovieDto>.Success(movieDto);
            }
            catch (Exception ex) {
                return OperationDetails<MovieDto>.Failure().AddError(ex.Message);
            }
        }

        // Get genres of movie
        public async Task<OperationDetails<IEnumerable<GenreDto>>> GetGenresOfMovieAsync(Guid movieId) {
            try {
                var genres = await _unitOfWork.Movies.GetGenresByMovieAsync(movieId);
                var dto = _mapper.Map<List<GenreDto>>(genres);
                return OperationDetails<IEnumerable<GenreDto>>.Success(dto);
            }
            catch(Exception ex) {
                return OperationDetails<IEnumerable<GenreDto>>.Failure().AddError(ex.Message);
            }
        }

        // Get countries of movie
        public async Task<OperationDetails<IEnumerable<CountryDto>>> GetCountriesOfMovieAsync(Guid movieId) {
            try {
                var countries = await _unitOfWork.Movies.GetCountriesByMovieAsync(movieId);
                var dto = _mapper.Map<List<CountryDto>>(countries);
                return OperationDetails<IEnumerable<CountryDto>>.Success(dto);
            }
            catch (Exception ex) {
                return OperationDetails<IEnumerable<CountryDto>>.Failure().AddError(ex.Message);
            }
        }

        // Get artists of movie
        public async Task<OperationDetails<IEnumerable<ArtistDto>>> GetArtistsByCareerOfMovieAsync(Guid movieId, CareerEnum career) {
            try {
                var artists = await _unitOfWork.Movies.GetArtistsByMovieAndCareerAsync(movieId, career);
                var dto = _mapper.Map<List<ArtistDto>>(artists);
                return OperationDetails<IEnumerable<ArtistDto>>.Success(dto);
            }
            catch (Exception ex) {
                return OperationDetails<IEnumerable<ArtistDto>>.Failure().AddError(ex.Message);
            }
        }

        // Add genre to movie
        public async Task<OperationDetails<bool>> AddGenreAsync(Guid movieId, Guid genreId) {
            try {
                var isExists = await _unitOfWork.Movies.IsInGenreAsync(movieId, genreId);
                if(!isExists) {
                    await _unitOfWork.Movies.AddToGenreAsync(new MovieGenre() { MovieId = movieId, GenreId = genreId });
                }
                await _unitOfWork.CompleteAsync();
                return OperationDetails<bool>.Success(true);
            }
            catch(Exception ex) {
                return OperationDetails<bool>.Failure().AddError(ex.Message);
            }
        }

        // Remove genre from movie
        public async Task<OperationDetails<bool>> RemoveGenreAsync(Guid movieId, Guid genreId) {
            try {
                var isExists = await _unitOfWork.Movies.IsInGenreAsync(movieId, genreId);
                if (isExists) {
                    _unitOfWork.Movies.RemoveFromGenre(new MovieGenre() { MovieId = movieId, GenreId = genreId });
                    await _unitOfWork.CompleteAsync();
                }
                return OperationDetails<bool>.Success(true);
            }
            catch (Exception ex) {
                return OperationDetails<bool>.Failure().AddError(ex.Message);
            }
        }

        // Add country of movie
        public async Task<OperationDetails<bool>> AddCountryAsync(Guid movieId, Guid countryId) {
            try {
                var isExists = await _unitOfWork.Movies.IsInCountryAsync(movieId, countryId);
                if (!isExists) {
                    await _unitOfWork.Movies.AddToCountryAsync(new MovieCountry() { MovieId = movieId, CountryId = countryId });
                }
                await _unitOfWork.CompleteAsync();
                return OperationDetails<bool>.Success(true);
            }
            catch (Exception ex) {
                return OperationDetails<bool>.Failure().AddError(ex.Message);
            }
        }

        // Remove country from movie
        public async Task<OperationDetails<bool>> RemoveCountryAsync(Guid movieId, Guid countryId) {
            try {
                var isExists = await _unitOfWork.Movies.IsInCountryAsync(movieId, countryId);
                if (isExists) {
                    _unitOfWork.Movies.RemoveFromCountry(new MovieCountry() { MovieId = movieId, CountryId = countryId });
                    await _unitOfWork.CompleteAsync();
                }
                return OperationDetails<bool>.Success(true);
            }
            catch (Exception ex) {
                return OperationDetails<bool>.Failure().AddError(ex.Message);
            }
        }

        // Add artist to movie
        public async Task<OperationDetails<bool>> SaveArtistAsync(Guid movieId, MovieArtistDto artist) {
            try {
                // Find movie
                var movie = await _unitOfWork.Movies.GetByIdAsync(movieId);
                if (movie == null)
                    throw new Exception($"Movie with Id {movieId} was not found");

                // Find movie artist
                MovieArtist movieArtist = await _unitOfWork.Movies.GetByMovieAndArtistAndCareerAsync(movieId, artist.ArtistId, artist.CareerId);
                // If not exists
                if (movieArtist == null) {
                    // Create new movie artist
                    movieArtist = _mapper.Map<MovieArtist>(artist);
                    movieArtist.MovieId = movieId;
                    await _unitOfWork.Movies.AddToArtistAndCareerAsync(movieArtist);
                }
                else {
                    // Update existing movie artist
                    movieArtist = _mapper.Map(artist, movieArtist);
                    _unitOfWork.Movies.UpdateArtist(movieArtist);
                }

                // Save changes
                await _unitOfWork.CompleteAsync();
                return OperationDetails<bool>.Success(true);
            }
            catch (Exception ex) {
                return OperationDetails<bool>.Failure().AddError(ex.Message);
            }
        }

        // Remove artist from movie
        public async Task<OperationDetails<bool>> RemoveArtistAsync(Guid movieId, MovieArtistDto artist) {
            try {
                var movieArtist = await _unitOfWork.Movies.GetByMovieAndArtistAndCareerAsync(movieId, artist.ArtistId, artist.CareerId);

                if (movieArtist == null)
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
