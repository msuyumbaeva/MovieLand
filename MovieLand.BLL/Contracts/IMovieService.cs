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
        Task<OperationDetails<MovieDto>> CreateAsync(MovieCreateDto movieDto);
        Task<OperationDetails<bool>> SetGenres(Guid movieId, ICollection<Guid> genres);
        Task<OperationDetails<bool>> SetCountries(Guid movieId, ICollection<Guid> countries);
        Task<OperationDetails<DataTablesPagedResults<MovieListItemDto>>> GetAsync(DataTablesParameters table);
        Task<OperationDetails<MovieDto>> GetById(Guid id);
        Task<OperationDetails<bool>> AddArtist(Guid movieId, MovieArtistDto artist);
        Task<OperationDetails<bool>> RemoveArtist(Guid movieId, MovieArtistDto artist);
        Task<OperationDetails<IEnumerable<GenreDto>>> GetGenresOfMovie(Guid movieId);
        Task<OperationDetails<IEnumerable<CountryDto>>> GetCountriesOfMovie(Guid movieId);
        Task<OperationDetails<IEnumerable<ArtistDto>>> GetArtistsByCareerOfMovie(Guid movieId, CareerEnum career);
    }
}
