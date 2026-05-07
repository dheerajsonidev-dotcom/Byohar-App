using Byohar.Application.Responses.Identity;
using Byohar.Shared.Wrapper;
using MediatR;

namespace Byohar.Application.Requests.Identity
{
    public class PermissionPagingRequest : PagedRequest, IRequest<Result<PaginatedResult<PermissionsResponse>>>
    {
    }
}
