using AutoMapper;
using MovieLand.BLL.Dtos.Movie;
using MovieLand.ViewModels.Movie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            MovieMapping();
        }

        private void MovieMapping() {
            CreateMap<MovieCreateViewModel, MovieCreateDto>();
        }
    }
}
