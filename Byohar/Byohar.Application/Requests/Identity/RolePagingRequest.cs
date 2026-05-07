
using Byohar.Application.Responses.Identity;
using Byohar.Shared.Wrapper;
using MediatR;

namespace Byohar.Application.Requests.Identity
{
    public class RolePagingRequest : PagedRequest, IRequest<Result<PaginatedResult<RoleResponse>>>
    {
        public Guid? UserId { get; set; }
    }
}
