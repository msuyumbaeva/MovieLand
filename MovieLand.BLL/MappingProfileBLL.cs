using AutoMapper;
using MovieLand.BLL.Dtos.Artist;
using MovieLand.BLL.Dtos.Comment;
using MovieLand.BLL.Dtos.Country;
using MovieLand.BLL.Dtos.Genre;
using MovieLand.BLL.Dtos.Movie;
using MovieLand.Data.Models;
using System;
using System.Collections.Generic;
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
        }

        private void MovieMapping() {
            CreateMap<MovieCreateDto, Movie>();
            CreateMap<Movie, MovieDto>();
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
                .ForMember(dest=>dest.UserName, opt=>opt.MapFrom(src=>src.User.UserName))
                .ReverseMap();
        }
    }
}
