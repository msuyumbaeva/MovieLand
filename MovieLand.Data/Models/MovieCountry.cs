using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MovieLand.Data.Models
{
    public class MovieCountry
    {
        public Guid MovieId { get; set; }

        public Guid CountryId { get; set; }

        public Movie Movie { get; set; }

        public Country Country { get; set; }
    }
}
