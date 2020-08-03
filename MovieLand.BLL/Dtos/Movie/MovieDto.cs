using MovieLand.BLL.Dtos.Country;
using MovieLand.BLL.Dtos.Genre;
using MovieLand.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace MovieLand.BLL.Dtos.Movie
{
    public class MovieDto
    {
        public MovieDto() {
            Genres = new List<GenreDto>();
            Countries = new List<CountryDto>();
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string OriginalName { get; set; }

        public short Duration { get; set; }

        public byte MinAge { get; set; }

        public int ReleaseYear { get; set; }

        public string Description { get; set; }

        public string Poster { get; set; }

        public ICollection<GenreDto> Genres { get; set; }

        public ICollection<CountryDto> Countries { get; set; }

        public ICollection<MovieArtistDto> Directors { get; set; }

        public ICollection<MovieArtistDto> Actors { get; set; }
    }
}
