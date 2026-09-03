
using Byohar.Application.Responses.Identity;
using Byohar.Shared.Wrapper;
using MediatR;

namespace Byohar.Application.Requests.Identity
{
    public class RolePermissionPagingRequest : PagedRequest, IRequest<Result<PaginatedResult<RolePermissionResponse>>>
    {
        public Guid? RoleId { get; set; }
    }
}
