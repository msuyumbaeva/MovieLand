using MovieLand.Data.ApplicationDbContext;
using MovieLand.Data.Contracts.Repositories;
using MovieLand.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MovieLand.Data.Repositories
{
    public class ArtistRepository : Repository<Artist>, IArtistRepository
    {
        public ArtistRepository(AppDbContext context) : base(context) {
        }

        public async Task<Artist> GetByIdAsync(Guid id) {
            return await DbSet.FindAsync(id);
        }
    }
}
