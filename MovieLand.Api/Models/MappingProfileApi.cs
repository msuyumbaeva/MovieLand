using AutoMapper;
using MovieLand.Api.Models.Movie;
using MovieLand.Api.Models.Omdb;
using MovieLand.BLL.Dtos.Genre;
using MovieLand.BLL.Dtos.Movie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Api.Models
{
    public class MappingProfileApi : Profile
    {
        public MappingProfileApi() {
            MovieMapping();
        }

        private void MovieMapping() {
            CreateMap<OmdbMovie, MovieSourceDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.ReleaseYear, opt => opt.MapFrom(src => int.Parse(src.Year)))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => short.Parse(src.Runtime.Split(new char[] { ' ' }).FirstOrDefault())))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Plot.Substring(0,500)))
                .ForMember(dest => dest.Poster, opt => opt.Ignore())
                .ForMember(dest => dest.Genres, opt => opt.MapFrom(src=>src.Genre.Split(", ", StringSplitOptions.None)))
                .ForMember(dest => dest.Countries, opt => opt.MapFrom(src => src.Country.Split(", ", StringSplitOptions.None)))
                .ForMember(dest => dest.Directors, opt => opt.MapFrom(src => src.Director.Split(", ", StringSplitOptions.None)))
                .ForMember(dest => dest.Actors, opt => opt.MapFrom(src => src.Actors.Split(", ", StringSplitOptions.None)));
            CreateMap<MovieSourceDto, MovieCreateDto>();
        }
    }
}
