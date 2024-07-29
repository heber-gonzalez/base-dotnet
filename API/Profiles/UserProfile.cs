using AutoMapper;

namespace API.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<EditUserDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Permissions, opt => opt.Ignore()) 
            .ForMember(dest => dest.Status, opt => opt.Ignore()) 
            .ForMember(dest => dest.Username, opt => opt.Ignore())
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<User, UserDto>();
        
    }
}
