using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieLand.BLL.DataAnnotaions;
using MovieLand.Data.Enums;
using MovieLand.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.ViewModels.Movie
{
    public class MovieCreateViewModel {
        public MovieCreateViewModel() {
            Artists = new List<NamedArray<Guid>>();
        }

        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string OriginalName { get; set; }

        [Required]
        [Range(0, short.MaxValue)]
        [Display(Name = "Duration (min)")]
        public short Duration { get; set; }

        [Range(0, 100)]
        [Display(Name = "Minimum age")]
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

        public Guid[] Countries { get; set; }

        public Guid[] Genres { get; set; }

        public List<NamedArray<Guid>> Artists { get; set; }
    }
}
