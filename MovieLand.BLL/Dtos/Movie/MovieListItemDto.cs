using System;
using System.Collections.Generic;
using System.Text;

namespace MovieLand.BLL.Dtos.Movie
{
    public class MovieListItemDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string OriginalName { get; set; }

        public short Duration { get; set; }

        public int ReleaseYear { get; set; }

        public string Poster { get; set; }
    }
}
