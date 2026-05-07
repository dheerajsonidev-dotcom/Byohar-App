using AutoMapper;
using Byohar.Application.Identity;
using Byohar.Application.Requests.Identity;
using Byohar.Application.Responses;
using Byohar.Application.Responses.Identity;
using Byohar.Domain.Entities.Identity;
using Byohar.Shared.Wrapper;

namespace Byohar.Infrastructure.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserResponse, ApplicationUser>().ReverseMap();

        //CreateMap<ApplicationUser, UserListResponse>()
        //    .ForMember(x => x.Roles,
        //    src=> src.MapFrom(x => 
        //    x.UserRoles.Select(x => x.Role.Name)))
        //    .ReverseMap();
        CreateMap<PaginatedResult<UserListResponse>, PaginatedResult<ApplicationUser>>().ReverseMap();

        CreateMap<UserViewProfileResponse, ApplicationUser>().ReverseMap();
        CreateMap<UserLoginDeviceHistory, LoginDeviceInfoResponse>().ReverseMap();
        CreateMap<UserPermissionRequest, UserPermission>().ReverseMap();
    }
}
