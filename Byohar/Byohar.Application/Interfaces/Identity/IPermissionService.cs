using Byohar.Application.Interfaces.Common;
using Byohar.Application.Requests.Identity;
using Byohar.Application.Responses.Identity;
using Byohar.Shared.Wrapper;

namespace Byohar.Application.Interfaces.Identity
{
    public interface IPermissionService:IService
    {
        Task<Result<PaginatedResult<PermissionsResponse>>> GetAllAsync(PermissionPagingRequest request);
    }
}
