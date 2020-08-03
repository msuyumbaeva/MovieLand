using MovieLand.BLL.Dtos.Artist;
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
        // Create artist
        Task<OperationDetails<ArtistDto>> CreateAsync(ArtistDto artist);
        // Edit artist
        Task<OperationDetails<ArtistDto>> EditAsync(ArtistDto artist);
    }
}
