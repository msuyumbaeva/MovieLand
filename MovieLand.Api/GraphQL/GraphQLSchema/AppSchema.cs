using GraphQL;
using GraphQL.Types;
using MovieLand.Api.GraphQL.GraphQLQueries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Api.GraphQL.GraphQLSchema
{
    public class AppSchema : Schema
    {
        public AppSchema(IDependencyResolver resolver)
        : base(resolver) {
            Query = resolver.Resolve<AppQuery>();
        }
    }
}
