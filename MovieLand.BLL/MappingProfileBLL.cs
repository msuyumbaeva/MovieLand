using AutoMapper;
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
        }

        private void MovieMapping() {
            CreateMap<MovieCreateDto, Movie>();
            CreateMap<Movie, MovieDto>();
            CreateMap<Movie, MovieListItemDto>();
        }

        private void GenreMapping() {
            CreateMap<Genre, GenreDto>()
                .ReverseMap();
        }

        private void CountryMapping() {
            CreateMap<Country, CountryDto>()
                .ReverseMap();
        }
    }
}
