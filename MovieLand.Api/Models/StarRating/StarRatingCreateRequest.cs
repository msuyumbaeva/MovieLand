using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Api.Models.StarRating
{
    public class StarRatingCreateRequest
    {
        [Required]
        [Range(1, 5)]
        public byte Value { get; set; }
    }
}
