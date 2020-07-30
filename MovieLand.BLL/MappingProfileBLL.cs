using AutoMapper;
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
        }

        private void MovieMapping() {
            CreateMap<MovieCreateDto, Movie>();
        }
    }
}
