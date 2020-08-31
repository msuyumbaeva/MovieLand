using GraphQL.Types;
using MovieLand.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Api.GraphQL.GraphQLTypes
{
    public class CountryType: ObjectGraphType<Country>
    {
        public CountryType() {
            Field(x => x.Id, type: typeof(IdGraphType)).Description("Id property from the country object.");
            Field(x => x.Name).Description("Name property from the country object.");
        }
    }
}
