using Microsoft.AspNetCore.Http;
using MovieLand.BLL.DataAnnotaions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MovieLand.BLL.Dtos.Movie
{
    public class MovieCreateDto  
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string OriginalName { get; set; }

        [Required]
        [Range(0,short.MaxValue)]
        public short Duration { get; set; }

        [Range(0, 100)]
        public byte MinAge { get; set; }

        [Required]
        [Range(1800, 2030)]
        public int ReleaseYear { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [FileExtension(Extensions = ".png,.jpg,.jpeg")]
        [FileSize(1024 * 1024)]
        public IFormFile Poster { get; set; }
    }
}
