using MovieLand.BLL.Dtos;
using MovieLand.BLL.Dtos.Artist;
using MovieLand.BLL.Dtos.Country;
using MovieLand.BLL.Dtos.DataTables;
using MovieLand.BLL.Dtos.Genre;
using MovieLand.BLL.Dtos.Movie;
using MovieLand.Data.Enums;
using MovieLand.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MovieLand.BLL.Contracts
{
    public interface IMovieService
    {
        Task<OperationDetails<MovieDto>> SaveAsync(MovieCreateDto movieDto);

        Task<OperationDetails<bool>> AddGenreAsync(Guid movieId, Guid genreId);
        Task<OperationDetails<bool>> RemoveGenreAsync(Guid movieId, Guid genreId);

        Task<OperationDetails<bool>> AddCountryAsync(Guid movieId, Guid countryId);
        Task<OperationDetails<bool>> RemoveCountryAsync(Guid movieId, Guid countryId);

        Task<OperationDetails<bool>> SaveArtistAsync(Guid movieId, MovieArtistDto artist);
        Task<OperationDetails<bool>> RemoveArtistAsync(Guid movieId, MovieArtistDto artist);

        Task<OperationDetails<DataTablesPagedResults<MovieListItemDto>>> GetAsync(DataTablesParameters table);
        Task<OperationDetails<MovieDto>> GetByIdAsync(Guid id);

        Task<OperationDetails<IEnumerable<GenreDto>>> GetGenresOfMovieAsync(Guid movieId);
        Task<OperationDetails<IEnumerable<CountryDto>>> GetCountriesOfMovieAsync(Guid movieId);
        Task<OperationDetails<IEnumerable<ArtistDto>>> GetArtistsByCareerOfMovieAsync(Guid movieId, CareerEnum career);

        Task<OperationDetails<DataTablesPagedResults<MovieListItemDto>>> GetByGenreAsync(Guid genreId, int length, int start);
        Task<OperationDetails<DataTablesPagedResults<MovieListItemDto>>> GetByCountryAsync(Guid countryId, int length, int start);
        Task<OperationDetails<DataTablesPagedResults<MovieListItemDto>>> GetByArtistAsync(Guid artistId, int length, int start);
    }
}
