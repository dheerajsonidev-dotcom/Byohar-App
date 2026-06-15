using AutoMapper;
using Byohar.Application.Identity;
using Byohar.Application.Requests.Events;
using Byohar.Application.Requests.Identity;
using Byohar.Application.Requests.Tenant;
using Byohar.Application.Responses;
using Byohar.Application.Responses.Events;
using Byohar.Application.Responses.Identity;
using Byohar.Domain.Entities.Events;
using Byohar.Domain.Entities.Identity;
using Byohar.Domain.Entities.Tenant;
using Byohar.Shared.Wrapper;

namespace Byohar.Application.Mappings;

public class EntitiesProfile : Profile
{
    public EntitiesProfile()
    {








        #region Users
        CreateMap<ApplicationUser, UserResponse>();
        CreateMap<ApplicationUser, UserListResponse>()
        .ForMember(x => x.Roles,
            src => src.MapFrom(x =>
            x.UserRoles.Select(x => x.Role.Name)));


        CreateMap<ApplicationUser, UserRolesResponse>();

        CreateMap<UserRole, UserRoleModel>()
            .ForMember(dest => dest.RoleName, opt =>
         opt.MapFrom(src => src.Role.Name))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Role.Id))
            .ForMember(dest => dest.RoleDescription, opt => opt.MapFrom(src => src.Role.Description));




        #endregion

     
    

       
      

       

     

        #region  UserPermission
        CreateMap<UserPermissionRequest, UserPermission>();
        #endregion

        #region Event

        CreateMap<EventRequest, Event>();

        CreateMap<Event, EventResponse>();

        CreateMap<PaginatedResult<Event>,
            PaginatedResult<EventResponse>>();

        #endregion









        #region Tenant

        CreateMap<CreateTenantRequest, Tenant>();

        #endregion

    }

    
}