using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MovieLand.Data.Models
{
    public class StarRating
    {
        public Guid MovieId { get; set; }

        public string UserId { get; set; }

        [Range(1, 5)]
        public byte Value { get; set; }

        public DateTime CreatedAt { get; set; }

        public Movie Movie { get; set; }

        public AppUser User { get; set; }
    }
}
