using GraphQL.Types;
using MovieLand.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Api.GraphQL.GraphQLTypes
{
    public class MovieArtistType : ObjectGraphType<MovieArtist>
    {
        public MovieArtistType() {
            Field(x => x.Artist, type: typeof(ArtistType)).Description("Artist property from the movie artist object.");
            Field(x => x.CareerId, type: typeof(CareerTypeEnumType)).Description("Enumeration for the career object.");
        }
    }
}
