using Microsoft.EntityFrameworkCore;
using MovieLand.BLL.Contracts;
using MovieLand.Data.ApplicationDbContext;
using MovieLand.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieLand.BLL.Services
{
    // GenreService class to manage genres
    public class GenreService : IGenreService
    {
        private readonly AppDbContext _context;

        public GenreService(AppDbContext context) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Create genre
        public async Task<OperationDetails<Genre>> CreateAsync(Genre genre) {
            try {
                genre.Name = genre.Name.ToLower();

                var genresWithSameName = await _context.Genres.Where(g => g.Name == genre.Name).CountAsync();
                if(genresWithSameName > 0) {
                    throw new Exception($"Genre with name {genre.Name} is already exists");
                }

                var genreEntry = await _context.Genres.AddAsync(genre);
                await _context.SaveChangesAsync();

                return OperationDetails<Genre>.Success(genreEntry.Entity);
            }
            catch (Exception ex) {
                return OperationDetails<Genre>.Failure().AddError(ex.Message);
            }
        }

        // Edit genre
        public async Task<OperationDetails<Genre>> EditAsync(Genre genre) {
            try {
                var dbGenre = await _context.Genres.FindAsync(genre.Id);
                if(dbGenre == null) {
                    throw new Exception($"Genre with Id {genre.Id} was not found");
                }

                genre.Name = genre.Name.ToLower();

                var genresWithSameName = await _context.Genres.Where(g => g.Name == genre.Name && g.Id != genre.Id).CountAsync();
                if (genresWithSameName > 0) {
                    throw new Exception($"Genre with name {genre.Name} is already exists");
                }

                dbGenre.Name = genre.Name;
                var genreEntry = _context.Genres.Update(dbGenre);
                await _context.SaveChangesAsync();

                return OperationDetails<Genre>.Success(genreEntry.Entity);
            }
            catch (Exception ex) {
                return OperationDetails<Genre>.Failure().AddError(ex.Message);
            }
        }

        // Get all genres
        public async Task<OperationDetails<IEnumerable<Genre>>> GetAllAsync() {

            try {
                var genres = await _context.Genres.ToListAsync();
                return OperationDetails<IEnumerable<Genre>>.Success(genres);
            }
            catch (Exception ex) {
                return OperationDetails<IEnumerable<Genre>>.Failure().AddError(ex.Message);
            }
        }

        // Get one genre by id
        public async Task<OperationDetails<Genre>> GetByIdAsync(Guid id) {
            try {
                var genre = await _context.Genres.FindAsync(id);
                return OperationDetails<Genre>.Success(genre);
            }
            catch (Exception ex) {
                return OperationDetails<Genre>.Failure().AddError(ex.Message);
            }
        }
    }
}
