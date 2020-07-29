using MovieLand.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MovieLand.BLL.Contracts
{
    // Interface of service managing genres
    public interface IGenreService
    {
        // Get all genres
        Task<OperationDetails<IEnumerable<Genre>>> GetAllAsync();
        // Get one genre by id
        Task<OperationDetails<Genre>> GetByIdAsync(int id);
        // Create genre
        Task<OperationDetails<Genre>> CreateAsync(Genre genre);
        // Edit genre
        Task<OperationDetails<Genre>> EditAsync(Genre genre);
    }
}
