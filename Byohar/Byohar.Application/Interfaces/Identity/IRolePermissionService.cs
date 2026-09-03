using Byohar.Application.Interfaces.Common;
using Byohar.Application.Requests.Identity;
using Byohar.Application.Responses.Identity;
using Byohar.Shared.Wrapper;

namespace Byohar.Application.Interfaces.Identity;

public interface IRolePermissionService : IService
{
    Task<Result<List<RolePermissionResponse>>> GetAllAsync();

    Task<int> GetCountAsync();

    Task<Result<RolePermissionResponse>> GetByIdAsync(int id);

    Task<Result<List<RolePermissionResponse>>> GetAllByRoleIdAsync(Guid roleId);

    Task<Result<bool>> CheckRoleHasPermissionAsync(string role, string permission);

    Task<Result<string>> SaveAsync(RolePermissionRequest request);

    Task<Result<string>> DeleteAsync(int id);

    Task<Result<List<RolePermissionResponse>>> GetPermissionsByRoleIdsAndUserId(UserRolePermissionRequest request);
    Task<Result<List<UserPermissionResponse>>> GetPermissions(string id);
}