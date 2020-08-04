using MovieLand.BLL.Dtos.DataTables;
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
        // Create or Edit genre
        Task<OperationDetails<GenreDto>> SaveAsync(GenreDto genre);

        Task<OperationDetails<DataTablesPagedResults<GenreDto>>> GetDataAsync(DataTablesParameters table);
    }
}
