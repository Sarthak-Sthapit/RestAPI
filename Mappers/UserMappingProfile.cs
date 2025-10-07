using AutoMapper;
using RestAPI.Models;
using RestAPI.Application.Commands;
using RestAPI.Application.Queries;

namespace RestAPI.Mappers
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            // User → Command/Query Results
            CreateMap<User, CreateUserResult.UserData>()
                .ForMember(dest => dest.MemberSince, opt => opt.MapFrom(src => src.CreatedAt));

            CreateMap<User, UpdateUserResult.UserData>()
                .ForMember(dest => dest.MemberSince, opt => opt.MapFrom(src => src.CreatedAt));

            CreateMap<User, GetUserResult.UserData>()
                .ForMember(dest => dest.MemberSince, opt => opt.MapFrom(src => src.CreatedAt));

            CreateMap<User, GetAllUsersResult.UserSummary>()
                .ForMember(dest => dest.MemberSince, opt => opt.MapFrom(src => src.CreatedAt));
                

            CreateMap<User, AuthenticateUserResult.UserInfo>()
                .ForMember(dest => dest.MemberSince, opt => opt.MapFrom(src => src.CreatedAt));
        }
    }
}