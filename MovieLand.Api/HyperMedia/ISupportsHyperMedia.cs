using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Api.HyperMedia
{
    /// <summary>
    /// Interface for models that support Hypermedia
    /// </summary>
    public interface ISupportsHyperMedia
    {
        /// <summary>
        /// A collection of hepermedia links
        /// </summary>
        List<HyperMediaLink> Links { get; set; }
    }
}
