using Byohar.Application.Identity;
using Byohar.Application.Interfaces.Common;
using Byohar.Application.Requests;
using Byohar.Application.Requests.Identity;
using Byohar.Application.Responses;
using Byohar.Application.Responses.Identity;
using Byohar.Application.Validators.Identity;
using Byohar.Shared.Wrapper;
using Microsoft.AspNetCore.Http;

namespace Byohar.Application.Interfaces.Identity;

public interface IUserService : IService
{
    Task<IResult<UserResponse>> RegisterUser(RegisterRequest request);
    Task<IResult<UserResponse>> UpdateUserDetail(UpdatePersonalDetailRequest request);
    Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest request, string origin);
    Task<Result<bool>> ResetPasswordAsync(ResetPasswordRequest request);
    Task<IResult> VerifyResetPasswordToken(VerifyResetPasswordTokenRequest request);
    Task<Result<List<UserResponse>>> GetAllAsync();
    Task<int> GetCountAsync();
    Task<IResult<UserResponse>> GetAsync(Guid userId);
    Task<IResult<UserViewProfileResponse>> GetUserViewProfile(Guid userId);
    Task<IResult> UpdateUser(UpdateUserRequest request);
    Task<IResult> UpdateUserTheme(string themeLayout, string themeMode, string sidebarMode, string iconSize, string requestId);
    Task<IResult> ToggleUserStatusAsync(ToggleUserStatusRequest request);
    Task<IResult> Activate(UserActivationRequest request);
    Task<IResult> DeActivate(UserDeActivationRequest request);
    Task<IResult<UserRolesResponse>> GetRolesAsync(string id);
    Task<IResult<string>> ConfirmEmailAsync(Guid userId, string code);
    Task<PaginatedResult<UserResponse>> GetUserByFilter(GridArgument gridArgument);
    Task<IResult> ChangePassword(TokenRequest request);
    Task<string> ExportToExcelAsync(string searchString = "");
    Task<IResult> DeleteMultiUser(ActivateDeactivateRequest request);
    Task<IResult> UpdateUserProfileAsync(UpdatePersonalDetailRequest request, IFormFileCollection profileImage);
    Task<IResult> GetLoginHistory(string userId);
    Task<Result<List<UserListResponse>>> GetUsers();
    Task<PaginatedResult<UserListResponse>> GetUsers(UserPagedRequest request);
    Task<IResult> UpdateUserRole(UserRoleRequest request);
    Task<Result<PaginatedResult<RoleResponse>>> GetUserRolesAsync(UserRolePagingRequest request);
    Task<Result<string>> CreateUserPermission(CreateUpdateUserPermissionRequest request);
    Task<Result<string>> DeleteMany(List<Guid> ids);
    Task<Result<int>> DeleteUserRoles(Guid userId, List<Guid>? roleIds);
    Task<Result<List<Guid>>> GetUserBuildings();
    Task<Result<List<Guid>>> GetUserSites();
    Task<bool> IsUserLoggedInAsync();
    string CreatePassword();
    Task<string> GenerateJwtAsync();
    Task<Result<bool>> IsUserDetailAlreadyExist(string email, string phoneNumber);
    string GenerateToken(string email, Guid tenantId);
}