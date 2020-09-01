using GraphQL;
using GraphQL.Types;
using MovieLand.Api.GraphQL.GraphQLQueries;
using MovieLand.Api.GraphQL.GraphQLTypes;
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
            Mutation = resolver.Resolve<AppMutation>();
            RegisterValueConverter(new ByteValueConverter());
        }
    }
}
