using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Api.HyperMedia
{
    public class HyperMediaFilterOptions
    {
        public List<IResponseEnricher> ObjectContentResponseEnricherList { get; set; } = new List<IResponseEnricher>();
    }
}
