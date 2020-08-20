using MovieLand.Api.Models.Movie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MovieLand.Api.Models.MovieSources
{
    public interface IMovieSource
    {
        string Name { get; }
        Task<MovieSourceDto> GetMovieAsync(HttpClient httpClient, string value);
        Task<object> SearchMovieAsync(HttpClient httpClient, string value, int page);
    }
}
