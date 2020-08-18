using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Api.HyperMedia
{
    /// <summary>
    /// Valid RFC 5988 relation types
    /// </summary>
    public sealed class RelationType
    {
        /// <summary>
        /// Relation to itself
        /// </summary>
        public const string self = "self";

        /// <summary>
        /// Navigation to the next page
        /// </summary>
        public const string next = "next";

        /// <summary>
        /// Navigation to the previous page
        /// </summary>
        public const string previous = "previous";

        /// <summary>
        /// Navigation to the first page
        /// </summary>
        public const string first = "first";

        /// <summary>
        /// Navigation to the last page
        /// </summary>
        public const string last = "last";
    }

    public sealed class HttpActionVerb
    {
        public const string GET = "GET";
        public const string POST = "POST";
        public const string PUT = "PUT";
        public const string DELETE = "DELETE";
        public const string PATCH = "PATCH";
    }

    /// <summary>
    /// Represents a HyperMedia Link
    /// </summary>
    public class HyperMediaLink
    {
        /// <summary>
        /// The relationship between the object being returned and the object described by the link. In this case "self" indicates that the link is a reference back to the object itself.
        /// Helper types can be obtained from the RelationType
        /// </summary>
        public string Rel { get; set; }

        /// <summary>
        /// The hyperlink (Href) for the object being described by the link in the form of a URI
        /// </summary>
        public string Href { get; set; }

        /// <summary>
        /// The type of HTTP request (Action) that can be sent to this URI.
        /// Action names can be provider by the HttpActionVerb Helper class
        /// </summary>
        public string Action { get; set; }
    }
}
