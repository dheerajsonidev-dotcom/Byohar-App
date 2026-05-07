using Byohar.Application.Interfaces.Common;
using Byohar.Application.Requests.Identity;
using Byohar.Application.Responses.Identity;
using Byohar.Shared.Wrapper;
using System.DirectoryServices.AccountManagement;

namespace Byohar.Application.Interfaces.Identity;

public interface ITokenService : IService
{
    Task<Result<TokenResponse>> LoginAsync(TokenRequest model);
    Task<Result<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest model);
    Task<Result<TokenResponse>> RemoveRefereshToken(RefreshTokenRequest model);
    Task<Result<TokenResponse>> RemoveLoginDevice(RemoveLoginDeviceRequest model);
    Task<Result<TokenResponse>> LoginWithWindows(UserPrincipal principle, UserDeviceInfo deviceInfo);
}