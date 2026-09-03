using AutoMapper;
using Byohar.Application.Requests.Identity;
using Byohar.Application.Responses.Identity;
using Byohar.Domain.Entities.Identity;
using Byohar.Shared.Wrapper;

namespace Byohar.Infrastructure.Mappings;

public class RoleProfile : Profile
{
    public RoleProfile()
    {
        CreateMap<RoleResponse, Role>().ReverseMap();
        CreateMap<RolePermission, RolePermissionResponse>()
            .ForMember(x => x.Description, src => src.MapFrom(r => r.ClaimValue))
            .ForMember(x => x.Value, src => src.MapFrom(r => r.ClaimValue))
            .ReverseMap();
        CreateMap<RolePermissionRequest, RolePermission>();
        CreateMap<Role, UserRoleModel>()
            .ForMember(x => x.Id, src => src.MapFrom(r => r.Id))
            .ForMember(x => x.RoleName, src => src.MapFrom(r => r.Name))
            .ForMember(x => x.RoleDescription, src => src.MapFrom(r => r.Description))
            .ReverseMap();
        CreateMap<PaginatedResult<Role>, PaginatedResult<RoleResponse>>();
        CreateMap<RolePermissionCreateUpdateRequest, RolePermission>();
        CreateMap<CreateUpdateRoleRequest, Role>()
           .ForMember(dest => dest.RolePermissions, opt => opt.MapFrom(src => src.RolePermissions));  
         
    }
}
