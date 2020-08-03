using MovieLand.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MovieLand.Data.Models
{
    public class MovieArtist
    {
        public Guid MovieId { get; set; }

        public Guid ArtistId { get; set; }

        public CareerEnum CareerId { get; set; }
        
        [Range(1,Byte.MaxValue)]
        public byte Priority { get; set; }

        public virtual Movie Movie { get; set; }

        public virtual Artist Artist { get; set; }

        public virtual Career Career { get; set; }
    }
}
