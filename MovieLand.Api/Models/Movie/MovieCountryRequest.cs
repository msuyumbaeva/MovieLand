using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Api.Models.Movie
{
    public class MovieCountryRequest
    {
        [Required]
        public Guid CountryId { get; set; }
    }
}
