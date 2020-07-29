using System;
using System.Collections.Generic;
using System.Text;

namespace MovieLand.Data.Models
{
    public class MovieGenre
    {
        public Guid MovieId { get; set; }

        public Guid GenreId { get; set; }

        public Movie Movie { get; set; }

        public Genre Genre { get; set; }
    }
}
