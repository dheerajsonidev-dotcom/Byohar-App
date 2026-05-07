using Byohar.Application.Interfaces.Common;
using Byohar.Application.Requests.Identity;
using Byohar.Application.Responses.Identity;
using Byohar.Shared.Wrapper;

namespace Byohar.Application.Interfaces.Identity
{
    public interface IRoleService : IService
    {
        Task<Result<PaginatedResult<RoleResponse>>> GetAllAsync(RolePagingRequest request);

        Task<int> GetCountAsync();

        Task<Result<RoleResponse>> GetByIdAsync(Guid id);

        Task<Result<string>> SaveAsync(RoleRequest request);

        Task<Result<string>> DeleteAsync(Guid id);

        Task<Result<PermissionResponse>> GetAllPermissionsAsync(Guid roleId);

        Task<Result<string>> UpdatePermissionsAsync(PermissionRequest request);

        Task<Result<string>> AddUpdateRolePermission(CreateUpdateRoleRequest request);

        Task<Result<string>> DeleteMany(List<Guid> ids);

    }
}