using MovieLand.BLL.Dtos.Genre;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieLand.BLL.Contracts
{
    // Interface of service managing genres
    public interface IGenreService
    {
        // Get all genres
        Task<OperationDetails<IEnumerable<GenreDto>>> GetAllAsync();
        // Get one genre by id
        Task<OperationDetails<GenreDto>> GetByIdAsync(Guid id);
        // Create genre
        Task<OperationDetails<GenreDto>> CreateAsync(GenreDto genre);
        // Edit genre
        Task<OperationDetails<GenreDto>> EditAsync(GenreDto genre);
    }
}
