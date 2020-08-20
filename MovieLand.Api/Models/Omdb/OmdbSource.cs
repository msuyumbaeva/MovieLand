using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Options;
using MovieLand.Api.Models.Movie;
using MovieLand.Api.Models.MovieSources;
using MovieLand.BLL.Dtos.Movie;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MovieLand.Api.Models.Omdb
{
    public class OmdbSource : IMovieSource
    {
        private readonly static string API_KEY = "apikey";
        private readonly static string ID_KEY = "i";
        private readonly static string PLOT_KEY = "plot=full";

        private readonly IMapper _mapper;
        private readonly OmdbConfiguration _configuration;

        public OmdbSource(IMapper mapper, OmdbConfiguration omdbConfiguration) {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = omdbConfiguration ?? throw new ArgumentNullException(nameof(omdbConfiguration));
        }

        public string Name {
            get {
                return _configuration.Name;
            }
        }

        public async Task<MovieSourceDto> GetMovieAsync(HttpClient httpClient, string value) {
            OmdbMovie omdbMovie = null;
            MovieSourceDto movie = null;
            // Build url
            var url = string.Format("{0}?{1}={2}&{3}&{4}={5}", _configuration.URL, API_KEY, _configuration.ApiKey, PLOT_KEY, ID_KEY, value);
            // Get movie
            using (var response = await httpClient.GetAsync(url)) {
                // Check status code
                if (!response.IsSuccessStatusCode)
                    throw new Exception();

                // Get content as string
                var responseContent = await response.Content.ReadAsStringAsync();
                // Deserialize to OmdbMovie object
                omdbMovie = JsonConvert.DeserializeObject<OmdbMovie>(responseContent);

                // Check error
                if (omdbMovie.Response == false.ToString())
                    throw new Exception(omdbMovie.Error);

                // Map OmdbMovie object to MovieSourceDto
                movie = _mapper.Map<MovieSourceDto>(omdbMovie);
            }

            // Get movie poster
            using (var response = await httpClient.GetAsync(omdbMovie.Poster)) {
                // Check status code
                if (!response.IsSuccessStatusCode)
                    throw new Exception();

                // Get content type of file
                var posterContentType = response.Content.Headers.GetValues("Content-Type").FirstOrDefault();
                // Map content type to extension
                var posterExtension = MimeTypeMap.GetExtension(posterContentType);

                // Get file bytes
                var bytes = await response.Content.ReadAsByteArrayAsync();
                // Set movie poster
                var fileStream = new MemoryStream(bytes);
                IFormFile file = new FormFile(fileStream, 0, bytes.Length, "poster", string.Concat(value, posterExtension));
                movie.Poster = file;
            }

            return movie;
        }
    }
}
