using System;
using System.ComponentModel.DataAnnotations;

namespace MovieLand.BLL.Dtos.Genre
{
    public class GenreDto
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    }
}
