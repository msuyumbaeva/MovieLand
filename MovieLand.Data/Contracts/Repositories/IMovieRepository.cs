using MovieLand.Data.Enums;
using MovieLand.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MovieLand.Data.Contracts.Repositories
{
    public interface IMovieRepository : IRepository<Movie>
    {
        Task<Movie> GetByIdAsync(Guid id);
        Task<Movie> GetFullByIdAsync(Guid id);

        Task<IEnumerable<Genre>> GetGenresByMovieAsync(Guid movieId);
        Task<int> CountMoviesByGenreAsync(Guid genreId);
        Task<bool> IsInGenreAsync(Guid movieId, Guid genreId);
        Task AddToGenreAsync(MovieGenre movieGenre);
        void RemoveFromGenre(MovieGenre movieGenre);

        Task<IEnumerable<Country>> GetCountriesByMovieAsync(Guid movieId);
        Task<int> CountMoviesByCountryAsync(Guid countryId);
        Task<bool> IsInCountryAsync(Guid movieId, Guid countryId);
        Task AddToCountryAsync(MovieCountry movieCountry);
        void RemoveFromCountry(MovieCountry movieCountry);

        Task<IEnumerable<MovieArtist>> GetArtistsByMovieAsync(Guid movieId);
        Task<IEnumerable<Artist>> GetArtistsByMovieAndCareerAsync(Guid movieId, CareerEnum career);
        Task<int> CountMoviesByArtistAsync(Guid artistId);
        Task<MovieArtist> GetByMovieAndArtistAndCareerAsync(Guid movieId, Guid artistId, CareerEnum career);
        Task AddToArtistAndCareerAsync(MovieArtist movieArtist);
        void RemoveFromArtistAndCareer(MovieArtist movieArtist);
        void UpdateArtist(MovieArtist movieArtist);
    }
}
