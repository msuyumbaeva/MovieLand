using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Api.Models.Omdb
{
    public class OmdbMovieList
    {
        public List<OmdbMovieListItem> Search { get; set; } = new List<OmdbMovieListItem>();
        public string TotalResults { get; set; }
        public string Response { get; set; }
        public string Error { get; set; }
    }
    
    public class OmdbMovieListItem
    {
        public string Title { get; set; }
        public string Year { get; set; }
        public string omdbID { get; set; }
        public string Type { get; set; }
        public string Poster { get; set; }
    }
}
