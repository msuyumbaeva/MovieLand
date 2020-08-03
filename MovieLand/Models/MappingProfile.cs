using AutoMapper;
using MovieLand.BLL.Dtos.Movie;
using MovieLand.BLL.Dtos.User;
using MovieLand.ViewModels.Account;
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
            UserMapping();
        }

        private void MovieMapping() {
            CreateMap<MovieCreateViewModel, MovieCreateDto>();
        }

        private void UserMapping() {
            CreateMap<RegisterViewModel, UserDto>();
        }
    }
}
