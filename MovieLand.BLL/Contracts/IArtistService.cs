using MovieLand.BLL.Dtos.Artist;
using MovieLand.BLL.Dtos.DataTables;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MovieLand.BLL.Contracts
{
    public interface IArtistService
    {
        // Get all artists
        Task<OperationDetails<IEnumerable<ArtistDto>>> GetAllAsync();
        // Get one artist by id
        Task<OperationDetails<ArtistDto>> GetByIdAsync(Guid id);
        // Create or Edit country
        Task<OperationDetails<ArtistDto>> SaveAsync(ArtistDto country);
        // Get countries by conditions
        Task<OperationDetails<DataTablesPagedResults<ArtistDto>>> GetAsync(DataTablesParameters table);
    }
}
