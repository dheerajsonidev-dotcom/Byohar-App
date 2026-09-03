using AutoMapper;
using Byohar.Application.Responses.Identity;
using Byohar.Domain.Entities.Identity;
using Byohar.Shared.Wrapper;

namespace Byohar.Infrastructure.Mappings
{
    public class PermissionProfile : Profile
    {
        public PermissionProfile()
        {
            CreateMap<PermissionsResponse, Permission>().ReverseMap();
            CreateMap<PaginatedResult<Permission>, PaginatedResult<PermissionsResponse>>();
        }
    }
}
