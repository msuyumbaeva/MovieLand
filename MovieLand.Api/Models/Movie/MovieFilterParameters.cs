using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Api.Models
{
    public class MovieFilterParameters
    {
        public string Search { get; set; }
        public Guid? Genre { get; set; }
        public Guid? Country { get; set; }
        public Guid? Artist { get; set; }
    }
}
