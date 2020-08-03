using MovieLand.BLL.Dtos.Artist;
using MovieLand.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MovieLand.BLL.Dtos.Movie
{
    public class MovieArtistDto
    {
        public MovieArtistDto(Guid artistId, CareerEnum career, byte priority) {
            ArtistId = artistId;
            CareerId = career;
            Priority = priority;
        }

        [Required]
        public Guid ArtistId { get; set; }

        [Required]
        public CareerEnum CareerId { get; set; }

        [Range(1, Byte.MaxValue)]
        public byte Priority { get; set; }

        public virtual ArtistDto Artist { get; set; }
    }
}
