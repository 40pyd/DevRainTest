using AutoMapper;
using TestApp.API.Dtos;
using TestApp.API.Models;

namespace TestApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>();
            CreateMap<User, UserForDetailedDto>();
            CreateMap<UserForRegisterDto, User>();
            CreateMap<Blog, BlogForListDto>();
            CreateMap<Blog, BlogForDetailedDto>();
            CreateMap<BlogForUpdateDto, Blog>();
            CreateMap<BlogForAddDto, Blog>();
            CreateMap<CommentForCreationDto, Comment>().ReverseMap();
            CreateMap<Comment, CommentForReturnDto>();
        }
    }
}