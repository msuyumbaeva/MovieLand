using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Api.Models.MovieSources
{
    public class MovieSourceOptions
    {
        public List<IMovieSource> MovieSourcesList { get; set; } = new List<IMovieSource>();
    }
}
