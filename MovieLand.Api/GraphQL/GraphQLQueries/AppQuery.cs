using AutoMapper;
using GraphQL;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using MovieLand.Api.GraphQL.GraphQLTypes;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Dtos.DataTables;
using MovieLand.BLL.Dtos.Movie;
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
        public AppQuery(IMovieService movieService) {
            Field<ListGraphType<MovieType>>(
               "movies",
               arguments: new QueryArguments(
                   new QueryArgument<StringGraphType> { Name = "search" },
                   new QueryArgument<StringGraphType> { Name = "genre" },
                   new QueryArgument<StringGraphType> { Name = "country" },
                   new QueryArgument<StringGraphType> { Name = "artist" },
                   new QueryArgument<IntGraphType> { Name = "limit", DefaultValue = 10 },
                   new QueryArgument<IntGraphType> { Name = "offset", DefaultValue = 0 },
                   new QueryArgument<StringGraphType> { Name = "order", DefaultValue = "Name" },
                   new QueryArgument<BooleanGraphType> { Name = "orderAsc", DefaultValue = true }
               ),
               resolve: context => ResolveMovies(context, movieService)
            );

            Field<MovieType>(
                "movie",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "movieId" }),
                resolve: context => ResolveMovie(context, movieService)
            );
        }

        private IEnumerable<MovieDto> ResolveMovies(ResolveFieldContext<object> context, IMovieService movieService) {
            var search = context.GetArgument<string>("search");
            var genre = context.GetArgument<string>("genre");
            var country = context.GetArgument<string>("country");
            var artist = context.GetArgument<string>("artist");
            var limit = context.GetArgument<int>("limit");
            var offset = context.GetArgument<int>("offset");
            var order = context.GetArgument<string>("order");
            var orderAsc = context.GetArgument<bool>("orderAsc");

            var genreId = Guid.Empty;
            if (genre != null && !Guid.TryParse(genre, out genreId)) {
                context.Errors.Add(new ExecutionError("Wrong value for genre (guid)"));
                return null;
            }

            var countryId = Guid.Empty;
            if (country != null && !Guid.TryParse(country, out countryId)) {
                context.Errors.Add(new ExecutionError("Wrong value for country (guid)"));
                return null;
            }

            var artistId = Guid.Empty;
            if (artist != null && !Guid.TryParse(artist, out artistId)) {
                context.Errors.Add(new ExecutionError("Wrong value for artist (guid)"));
                return null;
            }

            var dtColumns = new DTColumn[] {
                    new DTColumn() { Data = "Name", Name = "Name", Orderable = true, Searchable = true },
                    new DTColumn() { Data = "ReleaseYear", Name = "ReleaseYear", Orderable = true, Searchable = true }
                };

            DTOrder dtOrder = new DTOrder() {
                Column = 0,
                Dir = orderAsc ? DTOrderDir.ASC : DTOrderDir.DESC
            };

            if (order == dtColumns[1].Name)
                dtOrder.Column = 1;

            var parameters = new MovieDataTablesParameters() {
                Columns = dtColumns,
                Order = new DTOrder[1] {
                    dtOrder
                },
                Draw = 0,
                Start = offset,
                Length = limit,
                Search = new DTSearch() {
                    Value = search ?? "",
                    Regex = false
                },
                Genre = genreId != Guid.Empty ? genreId : (Guid?)null,
                Country = countryId != Guid.Empty ? countryId : (Guid?)null,
                Artist = artistId != Guid.Empty ? artistId : (Guid?)null
            };

            var result = movieService.GetAllAsync(parameters).Result;
            if (result.IsSuccess)
                return result.Entity;
            else {
                context.Errors.Add(new ExecutionError(result.Errors[0]));
                return null;
            }

        }

        private MovieDto ResolveMovie(ResolveFieldContext<object> context, IMovieService movieService) {
            if (!Guid.TryParse(context.GetArgument<string>("movieId"), out Guid id)) {
                context.Errors.Add(new ExecutionError("Wrong value for guid"));
                return null;
            }

            var result = movieService.GetLightByIdAsync(id).Result;

            if (result.IsSuccess)
                return result.Entity;
            else {
                context.Errors.Add(new ExecutionError(result.Errors[0]));
                return null;
            }
        }
    }
}
