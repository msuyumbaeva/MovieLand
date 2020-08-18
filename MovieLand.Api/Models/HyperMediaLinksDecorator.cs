using MovieLand.Api.HyperMedia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Api.Models
{
    public class HyperMediaLinksDecorator<T> : ISupportsHyperMedia
    {
        public HyperMediaLinksDecorator(T responseObject) {
            Data = responseObject;
        }

        public T Data { get; }
        public List<HyperMediaLink> Links { get; set; } = new List<HyperMediaLink>();
    }
}
