using MovieLand.Data.ApplicationDbContext;
using MovieLand.Data.Contracts.Repositories;
using MovieLand.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Data.Repositories
{
    public class GenreRepository : Repository<Genre>, IGenreRepository
    {
        public GenreRepository(AppDbContext context) : base(context) {
        }

        public async Task<Genre> GetByIdAsync(Guid id) {
            return await DbSet.FindAsync(id);
        }

        public IQueryable<Genre> GetByNamePart(string namePart) {
            return DbSet.Where(g => g.Name.Contains(namePart));
        }
    }
}
