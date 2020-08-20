using AutoMapper;
using MovieLand.BLL.Dtos.Artist;
using MovieLand.BLL.Dtos.Comment;
using MovieLand.BLL.Dtos.Country;
using MovieLand.BLL.Dtos.Genre;
using MovieLand.BLL.Dtos.Movie;
using MovieLand.BLL.Dtos.StarRating;
using MovieLand.Data.Enums;
using MovieLand.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MovieLand.BLL
{
    public class MappingProfileBLL : Profile
    {
        public MappingProfileBLL() {
            MovieMapping();
            GenreMapping();
            CountryMapping();
            ArtistMapping();
            CommentMapping();
            StarRatingMapping();
        }

        private void MovieMapping() {
            CreateMap<MovieCreateDto, Movie>();
            CreateMap<Movie, MovieDto>()
                .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.MovieGenres.Select(mg => mg.Genre)))
                .ForMember(dest => dest.Countries, opt => opt.MapFrom(src => src.MovieCountries.Select(mg => mg.Country)))
                .ForMember(dest => dest.Directors, opt => opt.MapFrom(src => src.MovieArtists.Where(a => a.CareerId == CareerEnum.Director).Select(mg => mg.Artist)))
                .ForMember(dest => dest.Actors, opt => opt.MapFrom(src => src.MovieArtists.Where(a => a.CareerId == CareerEnum.Actor).Select(mg => mg.Artist)));
            CreateMap<Movie, MovieListItemDto>();
            CreateMap<MovieArtistDto, MovieArtist>()
                .ReverseMap();
        }

        private void GenreMapping() {
            CreateMap<Genre, GenreDto>()
                .ReverseMap();
        }

        private void CountryMapping() {
            CreateMap<Country, CountryDto>()
                .ReverseMap();
        }

        private void ArtistMapping() {
            CreateMap<Artist, ArtistDto>()
                .ReverseMap();
        }

        private void CommentMapping() {
            CreateMap<Comment, CommentDto>()
                .ForMember(dest=>dest.User, opt=>opt.MapFrom(src=>src.User.UserName))
                .ReverseMap()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User));
        }

        private void StarRatingMapping() {
            CreateMap<StarRatingDto, StarRating>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User))
                .ReverseMap();
        }
    }
}
