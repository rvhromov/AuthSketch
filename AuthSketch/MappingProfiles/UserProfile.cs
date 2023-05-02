using AuthSketch.Entities;
using AuthSketch.Models.Identity;
using AuthSketch.Models.Users;
using AutoMapper;

namespace AuthSketch.MappingProfiles;

public sealed class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, MeResponse>(MemberList.None);
        CreateMap<User, GetUsersResponse>(MemberList.None);
    }
}
