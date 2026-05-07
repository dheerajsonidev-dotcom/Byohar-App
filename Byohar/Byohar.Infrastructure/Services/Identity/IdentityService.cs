using Byohar.Application.Configurations;
using Byohar.Application.Interfaces.Identity;
using Byohar.Application.Requests.Identity;
using Byohar.Application.Responses.Identity;
using Byohar.Domain.Entities.Identity;
using Byohar.Persistence.Contexts;
using Byohar.Shared.Constants.User;
using Byohar.Shared.Wrapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.DirectoryServices.AccountManagement;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static Byohar.Shared.Constants.Common.ApplicationConstants;
using MessageConstants = Byohar.Shared.Constants.User.MessageConstants;

namespace Byohar.Infrastructure.Services.Identity;

public class IdentityService : ITokenService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRolePermissionService _rolePermissionService;
    private readonly AppConfiguration _appConfig;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IStringLocalizer<IdentityService> _localizer;
    private readonly IUserService _userService;
    private readonly ApplicationDbContext _dbContext;

    public IdentityService(UserManager<ApplicationUser> userManager,
        IRolePermissionService rolePermissionService,
        IOptions<AppConfiguration> appConfig,
        SignInManager<ApplicationUser> signInManager,
        IStringLocalizer<IdentityService> localizer,
        IUserService userService,
        ApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _rolePermissionService = rolePermissionService;
        _appConfig = appConfig.Value;
        _signInManager = signInManager;
        _localizer = localizer;
        _userService = userService;
        _dbContext = dbContext;
    }

    public async Task<Result<TokenResponse>> LoginAsync(TokenRequest model)
    {
        var user = _userManager.Users.Where(w => w.UserName == model.UserName ).FirstOrDefault();
        var retMsg = "";

        if (user == null)
        {
            retMsg = _localizer[MessageConstants.UsernameOrPasswordIncorrect];
        }
        else
        {
            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);

            if (user.IsDeleted)
                retMsg = _localizer[MessageConstants.AccountDeleted];
            else if (!user.IsActive)
                retMsg = _localizer[MessageConstants.AccountDeactivated];
            else if (!passwordValid)
            {
                await _userManager.AccessFailedAsync(user);
                retMsg = _localizer[MessageConstants.UsernameOrPasswordIncorrect];
            }
            else if (await _userManager.GetLockoutEndDateAsync(user) >= DateTime.UtcNow)
            {
                retMsg = _localizer[MessageConstants.AccountBlocked];
            }
        }

        if (!string.IsNullOrEmpty(retMsg))
        {
            return await Result<TokenResponse>.FailAsync(retMsg);
        }

        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.ResetAccessFailedCountAsync(user);
        await _userManager.UpdateAsync(user);

        if (model.UserDeviceInfo != null)
        {
            var newLoginInfo = await SaveClientInfoAsync(user.Id, model.UserDeviceInfo);
        }

        var token = await GenerateJwtAsync(user);
        var userPermissions = await _rolePermissionService.GetPermissions(user.Id.ToString());

        var response = new TokenResponse
        {
            Token = token,
            RefreshToken = user.RefreshToken,
            Id = user.Id.ToString(),
            UserImageURL = user.ProfilePictureDataUrl,
            Permissions = userPermissions.Data,
            TenantId = user.TenantId
        };

        return await Result<TokenResponse>.SuccessAsync(response);
    }

    public async Task<Result<TokenResponse>> RemoveRefereshToken(RefreshTokenRequest model)
    {
        await _signInManager.SignOutAsync();

        var response = new TokenResponse { Token = null, RefreshToken = null, RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(-1) };

        return await Result<TokenResponse>.SuccessAsync(response);
    }

    public async Task<Result<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest model)
    {
        if (model is null)
        {
            return await Result<TokenResponse>.FailAsync(_localizer[MessageConstants.InvalidClientToken]);
        }

        var userPrincipal = GetPrincipalFromExpiredToken(model.Token);
        var userEmail = userPrincipal.FindFirstValue(ClaimTypes.Email);
        var user = await _userManager.FindByEmailAsync(userEmail);

        if (user == null)
            return await Result<TokenResponse>.FailAsync(_localizer[MessageConstants.UserNotFound]);

        if (user.RefreshToken != model.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            return await Result<TokenResponse>.FailAsync(_localizer[MessageConstants.InvalidClientToken]);

        var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
        user.RefreshToken = GenerateRefreshToken();
        await _userManager.UpdateAsync(user);

        var response = new TokenResponse { Token = token, RefreshToken = user.RefreshToken, RefreshTokenExpiryTime = user.RefreshTokenExpiryTime };
        return await Result<TokenResponse>.SuccessAsync(response);
    }

    private async Task<string> GenerateJwtAsync(ApplicationUser user)
    {
        return GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
    }

    private async Task<IEnumerable<Claim>> GetClaimsAsync(ApplicationUser user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        var rolePermissions = new List<Claim>();

        foreach (var role in roles)
        {
            rolePermissions.Add(new Claim(ClaimTypes.Role, role));
        }


        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Name, user.FirstName ?? string.Empty),
            new(ClaimTypes.Surname, user.LastName ?? string.Empty),
            new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty),
        }
        .Union(userClaims)
        .Union(rolePermissions);

        return claims;
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private static string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
    {
        var token = new JwtSecurityToken(
           claims: claims,
           expires: DateTime.UtcNow.AddDays(2),
           signingCredentials: signingCredentials);

        var tokenHandler = new JwtSecurityTokenHandler();
        var encryptedToken = tokenHandler.WriteToken(token);

        return encryptedToken;
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfig.Secret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = false
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
            StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException(_localizer[MessageConstants.InvalidToken]);
        }

        return principal;
    }

    private SigningCredentials GetSigningCredentials()
    {
        var secret = Encoding.UTF8.GetBytes(_appConfig.Secret);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }

    private async Task<UserLoginDeviceHistory> SaveClientInfoAsync(Guid userId, UserDeviceInfo tokenRequest)
    {
        var newLoginInfo = new UserLoginDeviceHistory
        {
            UserId = userId,
            Browser = tokenRequest.Browser,
            DeviceName = tokenRequest.Device,
            DeviceType = tokenRequest.DeviceType,
            DateTime = DateTime.UtcNow
        };

        await _dbContext.UserLoginDeviceHistories.AddAsync(newLoginInfo);
        await _dbContext.SaveChangesAsync();

        return newLoginInfo;
    }

    public async Task<Result<TokenResponse>> RemoveLoginDevice(RemoveLoginDeviceRequest model)
    {
        var records = _dbContext.UserLoginDeviceHistories.Where(e => e.UserId.ToString() == model.UserId && e.Id == model.LoginDeviceId);
        _dbContext.UserLoginDeviceHistories.RemoveRange(records);
        await _dbContext.SaveChangesAsync();

        var response = new TokenResponse { Token = null, RefreshToken = null, RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(-1) };

        return await Result<TokenResponse>.SuccessAsync(response);
    }

    public async Task<Result<TokenResponse>> LoginWithWindows(UserPrincipal domainUser, UserDeviceInfo deviceInfo)
    {
        if (domainUser != null)
        {
            var provider = UserLoginTypeConstants.Windows;
            var providerUserId = domainUser.Sid.ToString();

            var user = await _userManager.FindByLoginAsync(provider, providerUserId);

            user ??= await RegisterUser(domainUser, provider, providerUserId);

            var retMsg = "";
            if (user == null)
                retMsg = _localizer[MessageConstants.UserNotFound];
            else
            {
                if (user.IsDeleted)
                    retMsg = _localizer[MessageConstants.AccountDeleted];

                if (!user.IsActive)
                    retMsg = _localizer[MessageConstants.AccountDeactivated];

                if (await _userManager.GetLockoutEndDateAsync(user) >= DateTime.UtcNow)
                {
                    retMsg = _localizer[MessageConstants.AccountBlocked];
                }
            }

            if (!string.IsNullOrEmpty(retMsg))
            {
                return await Result<TokenResponse>.FailAsync(retMsg);
            }

            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.ResetAccessFailedCountAsync(user);
            await _userManager.UpdateAsync(user);

            var newLoginInfo = await SaveClientInfoAsync(user.Id, deviceInfo);

            var token = await GenerateJwtAsync(user);
            var response = new TokenResponse { Token = token, RefreshToken = user.RefreshToken };
            return await Result<TokenResponse>.SuccessAsync(response);
        }

        return await Result<TokenResponse>.FailAsync(_localizer[MessageConstants.ActiveDirectoryServiceNotFound]);
    }

    private async Task<ApplicationUser> RegisterUser(UserPrincipal dUser, string provider, string providerUserId)
    {
        var registerRequest = new RegisterRequest()
        {
            UserLoginType = UserLoginTypeConstants.Windows
        };

        if (dUser?.GivenName != null) registerRequest.FirstName = dUser.GivenName;
        if (dUser?.Surname != null) registerRequest.LastName = dUser.Surname;
        if (dUser?.EmailAddress != null) registerRequest.Email = dUser.EmailAddress;

        var result = await _userService.RegisterUser(registerRequest);

        if (result.Succeeded)
        {
            var user = await _userManager.FindByIdAsync(result.Data.Id.ToString());
            if (user != null)
            {
                await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, providerUserId, provider));
                await _userManager.AddToRoleAsync(user, RoleConstants.User);

                return user;
            }
        }
        return null;
    }
}