using GraphQL.Types;
using MovieLand.BLL.Dtos.Movie;
using MovieLand.Data.Contracts.Repositories;

namespace MovieLand.Api.GraphQL.GraphQLTypes
{
    public class MovieType : ObjectGraphType<MovieDto>
    {
        public MovieType(IUnitOfWork unitOfWork) {
            Field(x=>x.Id, type: typeof(IdGraphType)).Description("Id property from the movie object.");
            Field(x => x.Name).Description("Name property from the movie object.");
            Field(x => x.OriginalName).Description("Original name property from the movie object.");
            Field(x => x.Duration, type: typeof(IntGraphType)).Description("Duration property from the movie object.");
            Field(x => x.MinAge, type: typeof(IntGraphType)).Description("Minimum age property from the movie object.");
            Field(x => x.ReleaseYear, type: typeof(IntGraphType)).Description("Release year property from the movie object.");
            Field(x => x.Description).Description("Description property from the movie object.");
            Field(x => x.Poster).Description("Poster property from the movie object.");
            Field(x => x.AvgRating, type: typeof(FloatGraphType)).Description("Average rating property from the movie object.");
            Field<ListGraphType<GenreType>>(
                "genres",
                resolve: context => unitOfWork.Movies.GetGenresByMovieAsync(context.Source.Id)
            );
            Field<ListGraphType<CountryType>>(
                "countries",
                resolve: context => unitOfWork.Movies.GetCountriesByMovieAsync(context.Source.Id)
            );
            Field<ListGraphType<MovieArtistType>>(
                "artists",
                resolve: context => unitOfWork.Movies.GetArtistsByMovieAsync(context.Source.Id)
            );
        }
    }
}
