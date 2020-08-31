using AutoMapper;
using GraphQL.Types;
using MovieLand.Api.GraphQL.GraphQLTypes;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Dtos.DataTables;
using MovieLand.BLL.Extensions;
using MovieLand.Data.Builders;
using MovieLand.Data.Contracts.Repositories;
using MovieLand.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MovieLand.Api.GraphQL.GraphQLQueries
{
    public class AppQuery : ObjectGraphType
    { 
        public AppQuery(IUnitOfWork unitOfWork) {
            Field<ListGraphType<MovieType>>(
               "movies",
               arguments: new QueryArguments(
                   new QueryArgument<StringGraphType> { Name = "name" },
                   new QueryArgument<StringGraphType> { Name = "releaseYear" },
                   new QueryArgument<StringGraphType> { Name = "genre" },
                   new QueryArgument<StringGraphType> { Name = "country" },
                   new QueryArgument<StringGraphType> { Name = "artist" },
                   new QueryArgument<IntGraphType> { Name = "limit", DefaultValue = 10 },
                   new QueryArgument<IntGraphType> { Name = "offset", DefaultValue = 0 },
                   new QueryArgument<StringGraphType> { Name = "order", DefaultValue = "Name" },
                   new QueryArgument<BooleanGraphType> { Name = "orderAsc", DefaultValue = true }
               ),
               resolve: context => ResolveMovies(context, unitOfWork)
           );
        }

        private IEnumerable<Movie> ResolveMovies(ResolveFieldContext<object> context, IUnitOfWork unitOfWork) {
            var name = context.GetArgument<string>("name");
            var year = context.GetArgument<string>("releaseYear");
            var genre = context.GetArgument<string>("genre");
            var country = context.GetArgument<string>("country");
            var artist = context.GetArgument<string>("artist");
            var limit = context.GetArgument<int>("limit");
            var offset = context.GetArgument<int>("offset");
            var order = context.GetArgument<string>("order");
            var orderAsc = context.GetArgument<bool>("orderAsc");

            // Query building
            var queryBuilder = new EntityQueryBuilder<Movie>();

            // Filter
            Expression<Func<Movie, bool>> filter = m => true;
            if (!string.IsNullOrEmpty(name))
                filter = filter.CombineWithAndAlso(m => m.Name.Contains(name));

            if (int.TryParse(year, out int intYear))
                filter = filter.CombineWithAndAlso(m => m.ReleaseYear == intYear);

            if (Guid.TryParse(genre, out Guid genreId))
                filter = filter.CombineWithAndAlso(m => m.MovieGenres.Count(mg => mg.GenreId == genreId) > 0);

            if (Guid.TryParse(country, out Guid countryId))
                filter = filter.CombineWithAndAlso(m => m.MovieCountries.Count(mg => mg.CountryId == countryId) > 0);

            if (Guid.TryParse(artist, out Guid artistId))
                filter = filter.CombineWithAndAlso(m => m.MovieArtists.Count(mg => mg.ArtistId == artistId) > 0);

            queryBuilder.SetFilter(filter);

            // Order
            Expression<Func<Movie, object>> orderProperty = null;

            // Order property
            if (order == "Name")
                orderProperty = m => m.Name;
            else if (order == "ReleaseYear")
                orderProperty = m => m.ReleaseYear;

            if (orderProperty != null && order != null) {
                // Order direction
                if (orderAsc)
                    queryBuilder.SetOrderBy(m => m.OrderBy(orderProperty));
                else
                    queryBuilder.SetOrderBy(m => m.OrderByDescending(orderProperty));
            }

            // Limit
            queryBuilder.SetLimit(limit);
            // Offset
            queryBuilder.SetOffset(offset);

            // End Query building

            var movies = unitOfWork.Movies.GetAsync(queryBuilder).Result;
            return movies;
        }
    }
}
