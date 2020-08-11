using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MovieLand.BLL.Dtos.StarRating
{
    public class StarRatingDto
    {
        [Required]
        public Guid MovieId { get; set; }

        public string UserName { get; set; }

        [Range(1, 5)]
        public byte Value { get; set; }
    }
}
