using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Api.Models.Omdb
{
    public class OmdbConfiguration
    {
        public string URL { get; set; }
        public string ApiKey { get; set; }
        public string Name { get; set; }
    }
}
