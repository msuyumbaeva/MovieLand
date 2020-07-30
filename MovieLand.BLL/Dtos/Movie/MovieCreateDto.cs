using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MovieLand.BLL.Dtos.Movie
{
    public class MovieCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string OriginalName { get; set; }

        [Required]
        public short Duration { get; set; }

        public byte MinAge { get; set; }

        [Required]
        public int ReleaseYear { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public IFormFile Poster { get; set; }
    }
}
