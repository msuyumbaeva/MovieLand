using System;
using System.Collections.Generic;
using System.Text;

namespace MovieLand.BLL.Dtos.Movie
{
    public class MovieListDto {
        public MovieListDto(List<MovieListItemDto> items) {
            Items = items ?? throw new ArgumentNullException(nameof(items));
        }

        public List<MovieListItemDto> Items { get; set; }
        public int TotalAmount { get; set; }
        public Page Page { get; set; }
    }
}
