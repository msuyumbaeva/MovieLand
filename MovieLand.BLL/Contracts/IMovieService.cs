using MovieLand.BLL.Dtos;
using MovieLand.BLL.Dtos.Movie;
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
        Task<OperationDetails<MovieListDto>> GetAllAsync(Page page);
    }
}
