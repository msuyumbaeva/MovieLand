using GraphQL;
using GraphQL.Types;
using MovieLand.Api.GraphQL.GraphQLTypes;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Dtos.Movie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Api.GraphQL.GraphQLQueries
{
    public class AppMutation : ObjectGraphType
    {
        public AppMutation(IMovieService movieService) {
            Field<MovieType>(
            "createMovie",
            arguments: new QueryArguments(new QueryArgument<NonNullGraphType<MovieInputType>> { Name = "movie" }),
            resolve: context => {
                var movie = context.GetArgument<MovieCreateDto>("movie");
                var result = movieService.SaveAsync(movie).Result;
                if (result.IsSuccess)
                    return result.Entity;
                else {
                    context.Errors.Add(new ExecutionError(result.Errors[0]));
                    return null;
                }
            });

            Field<MovieType>(
            "updateMovie",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<MovieInputType>> { Name = "movie" },
                new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "movieId" }
            ),
            resolve: context => {
                var movieId = context.GetArgument<Guid>("movieId");
                var movie = context.GetArgument<MovieCreateDto>("movie");
                movie.Id = movieId;

                var result = movieService.SaveAsync(movie).Result;
                if (result.IsSuccess)
                    return result.Entity;
                else {
                    context.Errors.Add(new ExecutionError(result.Errors[0]));
                    return null;
                }
            });
        }
    }
}
