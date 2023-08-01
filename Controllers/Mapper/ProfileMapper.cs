using AutoMapper;
using LearnIndentityAndAuthorization.Controllers.Dtos;
using LearnIndentityAndAuthorization.Controllers.Responses.Auth;
using LearnIndentityAndAuthorization.Controllers.Responses.Users;
using LearnIndentityAndAuthorization.Helpers;
using LearnIndentityAndAuthorization.Models;

namespace LearnIndentityAndAuthorization.Controllers.Mapper;

public class ProfileMapper : Profile
{
    public ProfileMapper()
    {
        CreateMap<RegisterUserDto, ApplicationUser>()
            .ForMember
            (dest => dest.PasswordHash, options => options.MapFrom(src => src.Password
            ));
        CreateMap<LoginUserDto, ApplicationUser>()
            .ForMember
            (dest => dest.PasswordHash, options => options.MapFrom(src => src.Password
            ));

        CreateMap<ApplicationUser, UserResponse>()
            .ForMember
            (dest => dest.DisplayName, options => options.MapFrom(src => src.Name
            ));

        CreateMap<CreatePostDto, Post>();

        CreateMap<GoogleUserInfo, ApplicationUser>()
        .ForMember(dest => dest.Name, options => options.MapFrom(src => $"{src.FamilyName} {src.GivenName}"))
        .ForMember(dest => dest.Avatar, options => options.MapFrom(src => src.Picture))
        .ForMember(dest => dest.Email, options => options.MapFrom(src => src.Email))
        .ForMember(dest => dest.UserName, options => options.MapFrom(src => src.Email))
        // Mật khẩu mặc định là Google_email
        .ForMember(dest => dest.PasswordHash, options => options.MapFrom(src => $"Google_{src.Email}"));
    }
}