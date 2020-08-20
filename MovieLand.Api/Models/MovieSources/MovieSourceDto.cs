using Microsoft.AspNetCore.Http;
using MovieLand.BLL.Dtos.Movie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Api.Models.Movie
{
    public class MovieSourceDto : MovieCreateDto
    {
        public ICollection<string> Genres { get; set; }

        public ICollection<string> Countries { get; set; }

        public ICollection<string> Directors { get; set; }

        public ICollection<string> Actors { get; set; }
    }
}
