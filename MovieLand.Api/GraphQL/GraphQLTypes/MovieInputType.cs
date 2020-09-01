using GraphQL.Types;
using MovieLand.BLL.Dtos.Movie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Api.GraphQL.GraphQLTypes
{
    public class MovieInputType : InputObjectGraphType
    {
        public MovieInputType() {
            Name = "movieInput";
            Field<NonNullGraphType<StringGraphType>>("name");
            Field<NonNullGraphType<StringGraphType>>("originalName");
            Field<NonNullGraphType<IntGraphType>>("duration");
            Field<NonNullGraphType<ByteGraphType>>("minAge");
            Field<NonNullGraphType<IntGraphType>>("releaseYear");
            Field<NonNullGraphType<StringGraphType>>("description");
        }
    }
}
