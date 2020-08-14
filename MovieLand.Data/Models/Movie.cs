using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MovieLand.Data.Models
{
    public class Movie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

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
        [StringLength(100)]
        public string Poster { get; set; }

        public double AvgRating { get; set; }

        public virtual ICollection<MovieGenre> MovieGenres { get; set; }
        public virtual ICollection<MovieCountry> MovieCountries { get; set; }
        public virtual ICollection<MovieArtist> MovieArtists { get; set; }
    }
}
