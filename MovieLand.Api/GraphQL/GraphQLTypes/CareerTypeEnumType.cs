using GraphQL.Types;
using MovieLand.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Api.GraphQL.GraphQLTypes
{
    public class CareerTypeEnumType : EnumerationGraphType<CareerEnum>
    {
        public CareerTypeEnumType() {
            Name = "Career";
            Description = "Enumeration for the career type object.";
        }
    }
}
