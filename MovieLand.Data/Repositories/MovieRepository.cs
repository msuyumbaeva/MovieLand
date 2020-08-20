using Microsoft.EntityFrameworkCore;
using MovieLand.Data.ApplicationDbContext;
using MovieLand.Data.Contracts.Repositories;
using MovieLand.Data.Enums;
using MovieLand.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieLand.Data.Repositories
{
    public class MovieRepository : Repository<Movie>, IMovieRepository
    {
        public MovieRepository(AppDbContext context) : base(context) {
        }

        public Task AddToArtistAndCareerAsync(MovieArtist movieArtist) {
            return _context.MovieArtists
                .AddAsync(movieArtist);
        }

        public Task AddToCountryAsync(MovieCountry movieCountry) {
            return _context.MovieContries.AddAsync(movieCountry);
        }

        public Task AddToGenreAsync(MovieGenre movieGenre) {
            return _context.MovieGenres.AddAsync(movieGenre);
        }

        public async Task<int> CountMoviesByArtistAsync(Guid artistId) {
            return await _context.MovieArtists
                .Where(m => m.ArtistId == artistId)
                .Select(m => m.MovieId)
                .Distinct()
                .CountAsync();
        }

        public async Task<int> CountMoviesByCountryAsync(Guid countryId) {
            return await _context.MovieContries
                .Where(m => m.CountryId == countryId)
                .Select(m => m.MovieId)
                .Distinct()
                .CountAsync();
        }

        public async Task<int> CountMoviesByGenreAsync(Guid genreId) {
            return await _context.MovieGenres
                .Where(m => m.GenreId == genreId)
                .Select(m => m.MovieId)
                .Distinct()
                .CountAsync();
        }

        public async Task<IEnumerable<Artist>> GetArtistsByMovieAndCareerAsync(Guid movieId, CareerEnum career) {
            return await _context.MovieArtists
                .Include(m => m.Artist)
                .Where(m => m.MovieId == movieId && m.CareerId == career)
                .OrderBy(m => m.Priority)
                .Select(m => m.Artist)
                .ToListAsync();
        }

        public async Task<IEnumerable<MovieArtist>> GetArtistsByMovieAsync(Guid movieId) {
            return await _context.MovieArtists
                .Include(m => m.Artist)
                .Where(m => m.MovieId == movieId)
                .OrderBy(m => m.Priority)
                .ToListAsync();
        }

        public Task<Movie> GetByIdAsync(Guid id) {
            return DbSet.FindAsync(id);
        }

        public Task<Movie> GetFullByIdAsync(Guid id) {
            return DbSet
                .Include("MovieGenres.Genre")
                .Include("MovieCountries.Country")
                .Include("MovieArtists.Artist")
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<MovieArtist> GetByMovieAndArtistAndCareerAsync(Guid movieId, Guid artistId, CareerEnum career) {
            return await _context.MovieArtists
                .Where(m => m.MovieId == movieId && m.ArtistId == artistId && m.CareerId == career)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Country>> GetCountriesByMovieAsync(Guid movieId) {
            return await _context.MovieContries
                .Include(m => m.Country)
                .Where(m => m.MovieId == movieId)
                .Select(m => m.Country)
                .ToListAsync();
        }

        public async Task<IEnumerable<Genre>> GetGenresByMovieAsync(Guid movieId) {
            return await _context.MovieGenres
                .Include(m => m.Genre)
                .Where(m => m.MovieId == movieId)
                .Select(m => m.Genre)
                .ToListAsync();
        }

        public async Task<bool> IsInCountryAsync(Guid movieId, Guid countryId) {
            return (await _context.MovieContries
                .Where(m => m.MovieId == movieId && m.CountryId == countryId)
                .CountAsync()) > 0;
        }

        public async Task<bool> IsInGenreAsync(Guid movieId, Guid genreId) {
            return (await _context.MovieGenres
                .Where(m => m.MovieId == movieId && m.GenreId == genreId)
                .CountAsync()) > 0;
        }

        public void RemoveFromArtistAndCareer(MovieArtist movieArtist) {
            _context.MovieArtists.Remove(movieArtist);
        }

        public void RemoveFromCountry(MovieCountry movieCountry) {
            _context.MovieContries.Remove(movieCountry);
        }

        public void RemoveFromGenre(MovieGenre movieGenre) {
            _context.MovieGenres.Remove(movieGenre);
        }

        public void UpdateArtist(MovieArtist movieArtist) {
            _context.MovieArtists.Update(movieArtist);
        }
    }
}
