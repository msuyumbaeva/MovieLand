using AutoMapper;
using MovieLand.BLL.Dtos.Comment;
using MovieLand.BLL.Dtos.Movie;
using MovieLand.BLL.Dtos.User;
using MovieLand.ViewModels.Account;
using MovieLand.ViewModels.Comment;
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
            CommentMapping();
        }

        private void MovieMapping() {
            CreateMap<MovieCreateViewModel, MovieCreateDto>();
            CreateMap<MovieDto, MovieCreateViewModel>()
                .ForMember(dest=>dest.Poster, opt=>opt.Ignore())
                .ForMember(dest=>dest.Genres, opt=>opt.Ignore())
                .ForMember(dest=>dest.Countries, opt=>opt.Ignore())
                .ForMember(dest=>dest.Artists, opt=>opt.Ignore());
        }

        private void UserMapping() {
            CreateMap<RegisterViewModel, UserDto>();
        }

        private void CommentMapping() {
            CreateMap<CommentCreateViewModel, CommentDto>();
        }
    }
}
