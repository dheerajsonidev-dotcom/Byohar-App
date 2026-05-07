using AutoMapper;
using Byohar.Application.Configurations;
using Byohar.Application.Exceptions;
using Byohar.Application.Identity;
using Byohar.Application.Interfaces.Common;
using Byohar.Application.Interfaces.Email;
using Byohar.Application.Interfaces.Email.IEmailPopulate;
using Byohar.Application.Interfaces.Identity;
using Byohar.Application.Interfaces.Persistence;
using Byohar.Application.Interfaces.User;
using Byohar.Application.Models.Email;
using Byohar.Application.Requests.Identity;
using Byohar.Application.Requests.UploadFiles;
using Byohar.Application.Responses;
using Byohar.Application.Responses.Identity;
using Byohar.Application.Validators.Identity;
using Byohar.Domain.Entities.Identity;
using Byohar.Infrastructure.Extensions;
using Byohar.Infrastructure.Specifications;
using Byohar.Persistence.Contexts;
using Byohar.Shared.Constants.User;
using Byohar.Shared.Wrapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Byohar.Shared.Constants.Common.ApplicationConstants;
using IResult = Byohar.Shared.Wrapper.IResult;

namespace Byohar.Infrastructure.Services.Identity;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IStringLocalizer<UserService> _localizer;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEmailSender _mailService;
    private readonly IExcelService _excelService;
    private readonly IUploadService _uploadService;
    private readonly IDistributedCache _cache;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEmailPopulateBody _emailPopulateBody;
    private readonly AppConfiguration _appConfig;

    public UserService(
        UserManager<ApplicationUser> userManager,
        IMapper mapper,
        RoleManager<Role> roleManager,
        IStringLocalizer<UserService> localizer,
        ApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IEmailSender mailService,
        IExcelService excelService,
        IUploadService uploadService,
        IOptions<AppConfiguration> appConfig,
        IDistributedCache cache,
        IHttpContextAccessor httpContextAccessor,
        IEmailPopulateBody emailPopulateBody)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _userManager = userManager;
        _mapper = mapper;
        _roleManager = roleManager;
        _localizer = localizer;
        _dbContext = dbContext;
        _mailService = mailService;
        _excelService = excelService;
        _uploadService = uploadService;
        _cache = cache;
        _httpContextAccessor = httpContextAccessor;
        _emailPopulateBody = emailPopulateBody;
        _appConfig = appConfig.Value;
    }

    public async Task<Result<List<UserResponse>>> GetAllAsync()
    {
        var tenantId = GetTenantId();
        var users = await _userManager.Users.Where(x => !x.IsDeleted && x.IsActive && x.TenantId == tenantId).ToListAsync();
        var result = _mapper.Map<List<UserResponse>>(users);

        return await Result<List<UserResponse>>.SuccessAsync(result);
    }

    public async Task<IResult<UserResponse>> RegisterUser(RegisterRequest request)
    {
        try
        {
            string password;
            if (string.IsNullOrEmpty(request.Password))
                password = CreatePassword();
            else
                password = request.Password;

            string profileImageName = string.Empty;

            var user = new ApplicationUser
            {
                Id = request.Id ?? Guid.NewGuid(),
                //TenantId = request.TenantId.Value,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.Email,
                EmailConfirmed = request.EmailConfirmed,
                IsActive = request.IsActive,
            };

            if (request.TenantId.HasValue)
                user.TenantId = request.TenantId.Value;

            if (request.ProfilePicture?.FileName != null)
            {
                var profilePictureUrl = await _uploadService.UploadAsync(new UploadRequest
                {
                    Path = FileUploadUrl.Profile,
                    FileBytes = request.ProfilePicture.Content,
                    FileName = request.ProfilePicture.FileName
                });

                user.ProfilePictureDataUrl = profilePictureUrl.Data;
            }

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                var userWithSamePhoneNumber = await _userManager.Users.AnyAsync(x => x.PhoneNumber == request.PhoneNumber && !x.IsDeleted);

                if (userWithSamePhoneNumber)
                {
                    return await Result<UserResponse>.FailAsync(string.Format(_localizer["Phone number {0} is already registered."], request.PhoneNumber));
                }

                user.PhoneNumber = request.PhoneNumber;
            }

            var userWithSameEmail = await _userManager.Users.AnyAsync(x => request.Email != null && x.Email == request.Email && !x.IsDeleted);

            if (!userWithSameEmail)
            {
                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    AddRelatedEntities(user, request);
                    await _userManager.AddToRolesAsync(user, request.Roles);
                    await SendVerificationEmail(request.Email, password, request.Origin);

                    return await Result<UserResponse>.SuccessAsync(new UserResponse { Id = user.Id, UserName = user.UserName, Password = password }, string.Format(_localizer["User {0} Registered."], user.UserName));
                }
                else
                {
                    return await Result<UserResponse>.FailAsync(result.Errors.Select(a => _localizer[a.Description].ToString()).ToList());
                }
            }
            else
            {
                return await Result<UserResponse>.FailAsync(string.Format(_localizer["Email {0} is already registered."], request.Email));
            }
        }
        catch
        {
            return await Result<UserResponse>.FailAsync(string.Format(_localizer["Something went wrong"]));
        }

    }

    private void AddRelatedEntities(ApplicationUser user, RegisterRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<IResult> UpdateUser(UpdateUserRequest request)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == request.Id);

        if (user != null)
        {
            string profileImageName = string.Empty;

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;

            if (request.ProfilePicture?.FileName != null)
            {
                var profilePictureUrl = await _uploadService.UploadAsync(new UploadRequest
                {
                    Path = FileUploadUrl.Profile,
                    FileBytes = request.ProfilePicture.Content,
                    FileName = request.ProfilePicture.FileName
                });

                user.ProfilePictureDataUrl = profilePictureUrl.Data;
            }
            else
            {
                user.ProfilePictureDataUrl = null;
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return await Result<UserResponse>.SuccessAsync(new UserResponse { Id = user.Id, FirstName = request.FirstName, UserName = user.Email }, string.Format(_localizer["User {0} Registered."], user.UserName));
            }
            else
            {
                return await Result.FailAsync(result.Errors.Select(a => _localizer[a.Description].ToString()).ToList());
            }
        }

        return await Result.FailAsync(string.Format(_localizer["User Not Found!"]));
    }

    public async Task<IResult> UpdateUserTheme(string themeLayout, string themeMode, string sidebarMode, string iconSize, string requestId)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == new Guid(requestId));

        if (user != null)
        {
            //users.ThemeLayout = themeLayout;
            //users.ThemeMode = themeMode;
            //users.SidebarMode = sidebarMode;
            //users.IconSize = iconSize;

            //TODO - Will do if needed

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return await Result<UserResponse>.SuccessAsync(_localizer["UserTheme Updated Successfully"]);
            }
            else
            {
                return await Result.FailAsync(result.Errors.Select(a => _localizer[a.Description].ToString()).ToList());
            }
        }
        else
        {
            return await Result.FailAsync(_localizer["users Not Found"]);
        }
    }

    public async Task SendVerificationEmail(string userEmail, string password, string origin)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(userEmail);

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var loginPage = "auth/login";
            var resetUri = new Uri($"{origin}#/");
            var loginPageUrl = !origin.IsWhiteSpace() ? new Uri($"{origin}#/{loginPage}") : new Uri(loginPage);

            var confirmationLink = $"{resetUri}auth/verify/confirmEmail?userId={user.Id}&code={code}";

            string body = await _emailPopulateBody.PopulateBody("EmailConfirmTemplate.html");

            body = body.Replace("{userName}", user.UserName);
            body = body.Replace("{password}", password);
            body = body.Replace("{loginPage}", loginPageUrl.ToString());
            body = body.Replace("{confirmationLink}", confirmationLink);

            _mailService.SendEmail(new EmailMessage
            {
                To = user.Email,
                Body = body,
                Subject = "Verification Email"
            });
        }
        catch (Exception ex) { }
    }

    public async Task<IResult<UserResponse>> GetAsync(Guid userId)
    {
        var tenantId = GetTenantId();
        var user = _userManager.Users.Include(x => x.UserRoles).ThenInclude(x => x.Role)
                    .FirstOrDefault(w => w.Id == userId && w.TenantId == tenantId);

        if (user == null)
        {
            return await Result<UserResponse>.FailAsync();
        }

        var usermapped = _mapper.Map<UserResponse>(user);

        return await Result<UserResponse>.SuccessAsync(usermapped);
    }

    public async Task<IResult<UserViewProfileResponse>> GetUserViewProfile(Guid userId)
    {
        var tenantId = GetTenantId();
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId && u.TenantId == tenantId);

        if (user == null)
            return await Result<UserViewProfileResponse>.FailAsync(_localizer["User Not Found"]);

        UserViewProfileResponse usermapped = _mapper.Map<UserViewProfileResponse>(user);

        return await Result<UserViewProfileResponse>.SuccessAsync(usermapped);
    }

    public async Task<IResult> ToggleUserStatusAsync(ToggleUserStatusRequest request)
    {
        var user = await _userManager.Users.Where(u => u.Id == request.UserId).FirstOrDefaultAsync();
        if (user != null)
        {
            user.IsActive = !user.IsActive;
            var identityResult = await _userManager.UpdateAsync(user);
            return await Result<UserResponse>.SuccessAsync(new UserResponse { UserName = user.UserName, FirstName = user.FirstName, IsActive = user.IsActive });
        }

        return await Result.FailAsync(_localizer["User Not Found"]);
    }

    public async Task<IResult> MultiUserStatusToggel(ActivateDeactivateRequest request)
    {
        var selectedUser = _userManager.Users.Where(x => request.UserIds.Contains(x.Id)).ToList();
        foreach (var user in selectedUser)
        {
            user.IsActive = request.Activate;
        }
        _dbContext.Users.UpdateRange(selectedUser);
        await _dbContext.SaveChangesAsync();
        string text = request.Activate ? "Activated" : "Deactivated";

        return await Result.SuccessAsync(_localizer[$"{selectedUser.Count} users's successfully {text}."]);
    }

    public async Task<IResult> Activate(UserActivationRequest request)
    {
        var selectedUsers = _userManager.Users.Where(x => request.UserIds.Contains(x.Id)).Include(x => x.UserRoles).ThenInclude(x => x.Role).ToList();

        var role = await _roleManager.Roles.Where(x => x.Id == request.RoleId).SingleOrDefaultAsync();

        if (selectedUsers.Count == 0)
        {
            return await Result.FailAsync(_localizer[$"No users found"]);
        }

        foreach (var user in selectedUsers)
        {
            user.IsActive = true;
            _dbContext.Update(user);

            try
            {
                var userRole = user.UserRoles.ToList();

                if (userRole != null && userRole.Count > 0)
                {
                    _dbContext.UserRoles.RemoveRange(userRole);
                    await _dbContext.SaveChangesAsync();
                }

                await _userManager.AddToRoleAsync(user, role.Name);
                _dbContext.SaveChanges();
            }
            catch
            {
                return await Result.FailAsync(_localizer["Something went wrong"]);
            }
        }

        if (selectedUsers.Count > 1)
        {
            return await Result.SuccessAsync(_localizer[$"{selectedUsers.Count} users successfully Activated"]);

        }
        else
        {
            return await Result.SuccessAsync(_localizer[$"users successfully Activated"]);
        }

    }

    public async Task<IResult> DeActivate(UserDeActivationRequest request)
    {
        var selectedUser = _userManager.Users.Where(x => request.UserIds.Contains(x.Id)).ToList();

        foreach (var user in selectedUser)
        {
            //users
            user.IsActive = false;
            _dbContext.Update(user);

            var userRoles = await _dbContext.UserRoles.Where(x => x.UserId == user.Id).ToListAsync();

            if (userRoles.Count != 0)
            {
                foreach (var item in userRoles)
                {
                    var role = _roleManager.Roles.Where(x => x.Id == item.RoleId).SingleOrDefault();
                    await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
            }

            _dbContext.SaveChanges();
        }

        return await Result.SuccessAsync(_localizer[$"{selectedUser.Count} users's successfully De-Activated"]);
    }

    public async Task<IResult<UserRolesResponse>> GetRolesAsync(string userId)
    {
        var viewModel = new List<UserRoleModel>();
        var uId = new Guid(userId);
        var user = await _userManager.Users
            .Include(x => x.UserRoles).ThenInclude(r => r.Role).FirstOrDefaultAsync(x => x.Id == uId);
        if (user == null)
        {
            return await Result<UserRolesResponse>.FailAsync(_localizer[$"No users found."]);
        }
        var assignedRoleIds = user.UserRoles.Select(ur => ur.Role.Id).ToList();

        // Filter roles based on assigned roles
        var roles = await _roleManager.Roles
            .Where(role => assignedRoleIds.Contains(role.Id))
            .ToListAsync();

        // Build the view model with only the assigned roles
        foreach (var role in roles)
        {
            var userRolesViewModel = new UserRoleModel
            {
                RoleName = role.Name,
                RoleDescription = role.Description,
                Selected = true // All roles are assigned, so mark them as selected
            };

            viewModel.Add(userRolesViewModel);
        }

        var result = new UserRolesResponse { UserRoles = viewModel };
        return await Result<UserRolesResponse>.SuccessAsync(result);
    }

    public async Task<IResult<string>> ConfirmEmailAsync(Guid userId, string code)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user != null && user.EmailConfirmed)
        {
            if (user.IsActive)
            {
                return await Result<string>.FailAsync("Password already reset");
            }
            return await Result<string>.SuccessAsync(user.Id.ToString(), string.Format(_localizer["Your email has confirmed {0}"], user.Email));
        }

        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

        var result = await _userManager.ConfirmEmailAsync(user, code);

        if (result.Succeeded)
        {
            return await Result<string>.SuccessAsync(user.Id.ToString(), string.Format(_localizer["Your email has confirmed {0}"], user.Email));
        }
        else
        {
            throw new ApiException(string.Format(_localizer["An error occurred while confirming {0}"], user.Email));
        }
    }

    public async Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest request, string origin)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == request.Email && !x.IsDeleted);

        if (user == null || (user != null && !user.EmailConfirmed))
        {
            return await Result.SuccessAsync(_localizer[MessageConstants.PasswordResetInitiated]);
        }

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        var resetUri = new Uri($"{origin}#/");
        var confirmationLink = $"{resetUri}auth/verify/verfiyToken?userId={user.Id}&code={code}";

        string body = await _emailPopulateBody.PopulateBody("ForgotPasswordTemplate.html");
        body = body.Replace("{userName}", user.FirstName + " " + user.LastName);
        body = body.Replace("{confirmationLink}", confirmationLink);

        _mailService.SendEmail(new EmailMessage
        {
            To = request.Email,
            Body = body,
            Subject = "Password Reset Request"
        });

        return await Result.SuccessAsync(_localizer[MessageConstants.PasswordResetInitiated]);
    }

    public async Task<Result<bool>> ResetPasswordAsync(ResetPasswordRequest request)
    {
        try
        {
            //this time user name is user id
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == request.UserId && !x.IsDeleted);

            if (user == null)
            {
                // Don't reveal that the users does not exist
                return await Result<bool>.FailAsync(_localizer["An Error has occured!"]);
            }
            else if (!string.IsNullOrEmpty(request.CurrentPassword))
            {
                var passwordVerificationResult = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword);
                if (passwordVerificationResult == PasswordVerificationResult.Failed)
                {
                    return await Result<bool>.FailAsync(_localizer["Current password is not correct!"]);
                }
            }

            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, request.NewPassword);


            string body = await _emailPopulateBody.PopulateBody("ChangedPasswordTemplate.html");

            body = body.Replace("{userName}", user.FirstName + " " + user.LastName);

            _mailService.SendEmail(new EmailMessage
            {
                To = user.Email,
                Body = body,
                Subject = "Changed Password Email"
            });

            user.IsActive = true;
            await _userManager.UpdateAsync(user);
            return await Result<bool>.SuccessAsync();

        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<IResult> ChangePassword(TokenRequest request)
    {
        var user = await _dbContext.Users.Where(x => x.UserName == request.UserName).FirstOrDefaultAsync();

        if (user == null)
        {
            // Don't reveal that the users does not exist
            return await Result.FailAsync(_localizer["An Error has occured!"]);
        }

        user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, request.Password);

        _dbContext.Update(user);
        await _dbContext.SaveChangesAsync();

        return await Result.SuccessAsync();
    }

    

    public async Task<int> GetCountAsync()
    {
        var tenantId = GetTenantId();
        return await _userManager.Users.Where(x => x.TenantId == tenantId).CountAsync();
    }

    public async Task<string> ExportToExcelAsync(string searchString = "")
    {
        var userSpec = new UserFilterSpecification(searchString);
        var users = await _userManager.Users.Specify(userSpec).ToListAsync();

        var result = await _excelService.ExportAsync(users, sheetName: _localizer["Users"],
            mappers: new Dictionary<string, Func<ApplicationUser, object>>
            {
                    { _localizer["Id"], item => item.Id },
                    { _localizer["FirstName"], item => item.FirstName },
                    { _localizer["LastName"], item => item.LastName },
                    { _localizer["UserName"], item => item.UserName },
                    { _localizer["Email"], item => item.Email },
                    { _localizer["EmailConfirmed"], item => item.EmailConfirmed },
                    { _localizer["PhoneNumber"], item => item.PhoneNumber },
                    { _localizer["PhoneNumberConfirmed"], item => item.PhoneNumberConfirmed },
                    { _localizer["IsActive"], item => item.IsActive },
                    { _localizer["ProfilePictureDataUrl"], item => item.ProfilePictureDataUrl },
            });

        return result;
    }

    public async Task<IResult> DeleteMultiUser(ActivateDeactivateRequest request)
    {
        var selectedUser = await _userManager.Users.Where(x => request.UserIds.Contains(x.Id)).ToListAsync();

        if (selectedUser.Count == 0)
        {
            return await Result.FailAsync(_localizer[$"No users found."]);
        }

        selectedUser = selectedUser.Select(x => { x.IsDeleted = true; return x; }).ToList();

        _dbContext.Users.UpdateRange(selectedUser);
        _dbContext.SaveChanges();

        if (selectedUser.Count > 1)
        {
            return await Result.SuccessAsync(_localizer[$"{selectedUser.Count} users successfully deleted."]);
        }
        else
        {
            return await Result.SuccessAsync(_localizer[$"User deleted successfully."]);
        }
    }

    public string CreatePassword()
    {
        int length = 12;
        const string lower = "abcdefghijklmnopqrstuvwxyz";
        const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string number = "1234567890";
        const string special = "!@#$%^&*";

        var middle = length / 2;
        StringBuilder res = new StringBuilder();
        Random rnd = new Random();
        while (0 < length--)
        {
            if (middle == length)
            {
                res.Append(number[rnd.Next(number.Length)]);
            }
            else if (middle - 1 == length)
            {
                res.Append(special[rnd.Next(special.Length)]);
            }
            else
            {
                if (length % 2 == 0)
                {
                    res.Append(lower[rnd.Next(lower.Length)]);
                }
                else
                {
                    res.Append(upper[rnd.Next(upper.Length)]);
                }
            }
        }
        return res.ToString();
    }

    public async Task<IResult> UpdateUserProfileAsync(UpdatePersonalDetailRequest request, IFormFileCollection profileImage)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == request.Id);

        if (user == null)
        {
            return await Result.FailAsync(string.Format(_localizer["User Not Found!"]));
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Email = request.Email;
        user.PhoneNumber = request.PhoneNumber;

      

        var result = await _userManager.UpdateAsync(user);
        await _unitOfWork.SaveAsync();

        if (result.Succeeded)
        {
            return await Result<UserResponse>.SuccessAsync(new UserResponse { Id = user.Id, FirstName = request.FirstName, UserName = user.UserName }, string.Format(_localizer["User {0} Registered."], user.UserName));
        }
        else
        {
            return await Result.FailAsync(result.Errors.Select(a => _localizer[a.Description].ToString()).ToList());
        }
    }

    public async Task<IResult> GetLoginHistory(string userId)
    {
        var userLoginHistroies = await _dbContext.UserLoginDeviceHistories.Where(x => x.UserId.ToString() == userId).OrderByDescending(y => y.DateTime).ToListAsync();
        List<LoginDeviceInfoResponse> ucl = _mapper.Map<List<LoginDeviceInfoResponse>>(userLoginHistroies);
        return await Result<List<LoginDeviceInfoResponse>>.SuccessAsync(ucl);
    }

    public async Task<Result<List<UserListResponse>>> GetUsers()
    {
        var tenantId = GetTenantId();
        var users = _userManager.Users.Where(x => x.IsActive && !x.IsDeleted && x.TenantId == tenantId);

        return await Result<List<UserListResponse>>.SuccessAsync(_mapper.Map<List<UserListResponse>>(await users.ToListAsync()));
    }

    public async Task<IResult> VerifyResetPasswordToken(VerifyResetPasswordTokenRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user == null)
            return await Result<bool>.FailAsync("An error has occured!");

        var options = new IdentityOptions();

        request.Token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));

        var isValid = await _userManager.VerifyUserTokenAsync(user, options.Tokens.PasswordResetTokenProvider, "ResetPassword", request.Token);

        if (isValid)
            return await Result<string>.SuccessAsync(user.Id.ToString(), string.Format(_localizer["{0}"], user.Email));

        return await Result.FailAsync(_localizer["An Error has occurred!"]);
    }

    public async Task<PaginatedResult<UserListResponse>> GetUsers(UserPagedRequest request)
    {
        var tenantId = GetTenantId();
        var usersQuery = _userManager.Users
            .Include(x => x.UserRoles).ThenInclude(x => x.Role)
            .Where(x => !x.IsDeleted && x.TenantId == tenantId)
            .WhereIf(!request.StringSearch.IsWhiteSpace(),
                user => user.FirstName.Contains(request.StringSearch) || user.LastName.Contains(request.StringSearch) || user.Email.Contains(request.StringSearch));

       

        if (request.IsAdmin == true)
        {
            usersQuery = usersQuery.Where(user =>
                user.UserRoles.Any(ur => ur.Role.Name == "Administrator")
            );
        }

        

        if (!string.IsNullOrEmpty(request.SortColumn))
        {
            usersQuery = usersQuery.ApplySorting(request.SortColumn, request.SortOrder);
        }

        var paginatedResult = await usersQuery.ToPaginatedListAsync(request.PageNumber, request.PageSize);
        var userList = _mapper.Map<PaginatedResult<UserListResponse>>(paginatedResult);

        return userList;
    }

    public async Task<IResult<UserResponse>> UpdateUserDetail(UpdatePersonalDetailRequest request)
    {
        try
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (user == null)
            {
                return await Result<UserResponse>.FailAsync(_localizer["User not found"]);
            }
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email ?? user.Email;
            user.PhoneNumber = request.PhoneNumber ?? user.PhoneNumber;
            user.Description = request.Description ?? user.Description;
            user.TimeZone = request.TimeZone ?? user.TimeZone;
            string profileImageName = string.Empty;
            if (request.ProfilePicture?.FileName != null)
            {
                var profilePictureUrl = await _uploadService.UploadAsync(new UploadRequest
                {
                    Path = FileUploadUrl.Profile,
                    FileBytes = request.ProfilePicture.Content,
                    FileName = request.ProfilePicture.FileName
                });

                user.ProfilePictureDataUrl = profilePictureUrl.Data;
            }

            var result = await _userManager.UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            if (result.Succeeded)
            {
                return await Result<UserResponse>.SuccessAsync(new UserResponse { Id = user.Id, FirstName = request.FirstName, UserName = user.UserName }, string.Format(_localizer["User {0} Update Successfully."], user.UserName));
            }
            else
            {
                return await Result<UserResponse>.FailAsync($"User {request.Id} not found");
            }
        }
        catch
        {
            return await Result<UserResponse>.FailAsync(string.Format(_localizer["Something went wrong"]));
        }

    }

    

    public async Task<IResult> UpdateUserRole(UserRoleRequest request)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == request.Id);

        if (user == null)
        {
            return await Result<UserResponse>.FailAsync($"User {request.Id} not found");
        }

        await _userManager.AddToRoleAsync(user, request.RoleName);

        var cachedPermissions = await _cache.GetStringAsync(user.Id.ToString());
        if (!string.IsNullOrEmpty(cachedPermissions))
        {
            await _cache.RemoveAsync(user.Id.ToString());
        }

        return await Result<UserResponse>.SuccessAsync(new UserResponse { Id = user.Id, FirstName = request.RoleName, UserName = user.Email }, string.Format(_localizer["Role {0} Registered."], user.UserName));
    }

    public async Task<Result<PaginatedResult<RoleResponse>>> GetUserRolesAsync(UserRolePagingRequest request)
    {
        var user = await _userManager.Users
            .Include(x => x.UserRoles).ThenInclude(r => r.Role)
            .FirstOrDefaultAsync(x => x.Id == request.UserId);

        if (user == null)
        {
            return await Result<PaginatedResult<RoleResponse>>.FailAsync("User not found");
        }

        var result = _mapper.Map<List<Role>, List<RoleResponse>>(user.UserRoles.Select(x => x.Role).ToList());

        if (!string.IsNullOrEmpty(request.SortColumn))
        {
            result = result.ApplySortingToList(request.SortColumn, request.SortOrder);
        }

        return await Result<PaginatedResult<RoleResponse>>.SuccessAsync(result.ToPaginatedEnumerableList(request.PageNumber, request.PageSize));
    }

    public async Task<Result<string>> CreateUserPermission(CreateUpdateUserPermissionRequest request)
    {
        try
        {
            if (request.UserId == Guid.Empty)
            {
                return await Result<string>.FailAsync(_localizer["User is required."]);
            }

            var existingPermission = _dbContext.UserPermissions.Where(x => x.UserId == request.UserId).ToList();

            if (existingPermission.Any())
                _dbContext.UserPermissions.RemoveRange(existingPermission);

            var userPermissions = _mapper.Map<List<UserPermissionRequest>, List<UserPermission>>(request.UserPermissionRequests);

            await _dbContext.UserPermissions.AddRangeAsync(userPermissions);
            await _dbContext.SaveChangesAsync();

            var cachedPermissions = await _cache.GetStringAsync(request.UserId.ToString());
            if (!string.IsNullOrEmpty(cachedPermissions))
            {
                await _cache.RemoveAsync(request.UserId.ToString());
            }

            return await Result<string>.SuccessAsync(string.Format(_localizer["User  Updated."]));

        }
        catch (Exception ex)
        {
            return await Result<string>.FailAsync(ex.Message);
        }
    }

    

    private void UpdateUserSites(ApplicationUser user, List<UserSiteRequest> newSites)
    {
        var existingSites = _unitOfWork.Repository<UserSite>().Entities()
                              .Where(userSite => userSite.UserId == user.Id)
                              .ToList();

        var sitesToRemove = existingSites.Where(techSite => !newSites.Any(ns => ns.SiteId == techSite.SiteId)).ToList();
        var sitesToAdd = newSites.Where(ns => !existingSites.Any(es => es.SiteId == ns.SiteId))
                                 .Select(ns => new UserSite { UserId = user.Id, SiteId = ns.SiteId })
                                 .ToList();

        if (sitesToRemove.Any()) _unitOfWork.Repository<UserSite>().DeleteRange(sitesToRemove);
        if (sitesToAdd.Any()) _unitOfWork.Repository<UserSite>().AddRange(sitesToAdd);
    }

    private void UpdateUserBuildings(ApplicationUser user, List<UserBuildingRequest> newBuildings)
    {
        var existingBuildings = _unitOfWork.Repository<UserBuilding>().Entities()
                                 .Where(userBuilding => userBuilding.UserId == user.Id)
                                 .ToList();

        var buildingsToRemove = existingBuildings.Where(techBuilding => !newBuildings.Any(nb => nb.BuildingId == techBuilding.BuildingId)).ToList();
        var buildingsToAdd = newBuildings.Where(nb => !existingBuildings.Any(eb => eb.BuildingId == nb.BuildingId))
                                         .Select(nb => new UserBuilding { UserId = user.Id, BuildingId = nb.BuildingId })
                                         .ToList();

        if (buildingsToRemove.Any()) _unitOfWork.Repository<UserBuilding>().DeleteRange(buildingsToRemove);
        if (buildingsToAdd.Any()) _unitOfWork.Repository<UserBuilding>().AddRange(buildingsToAdd);
    }

    

    public async Task<Result<string>> DeleteMany(List<Guid> ids)
    {
        var selectedUser = await _userManager.Users.Where(x => ids.Contains(x.Id)).ToListAsync();

        if (selectedUser.Count == 0)
        {
            return await Result<string>.FailAsync(_localizer[$"No users found."]);
        }

        selectedUser = selectedUser.Select(x => { x.IsDeleted = true; return x; }).ToList();

        _dbContext.Users.RemoveRange(selectedUser);

        _dbContext.SaveChanges();

        if (selectedUser.Count > 1)
        {
            return await Result<string>.SuccessAsync(_localizer[$"{selectedUser.Count} users successfully deleted."]);
        }
        else
        {
            return await Result<string>.SuccessAsync(_localizer[$"User deleted successfully."]);
        }
    }

    public async Task<Result<int>> DeleteUserRoles(Guid userId, List<Guid>? roleIds)
    {
        var user = await _userManager.Users
           .Include(x => x.UserRoles).ThenInclude(r => r.Role)
           .FirstOrDefaultAsync(x => x.Id == userId);

        var userRoles = user.UserRoles;

        if (roleIds?.Count > 0)
        {
            userRoles = userRoles.Where(x => roleIds.Contains(x.RoleId)).ToList();
        }

        var cachedPermissions = await _cache.GetStringAsync(userId.ToString());
        if (!string.IsNullOrEmpty(cachedPermissions))
            await _cache.RemoveAsync(userId.ToString());
        

        _dbContext.UserRoles.RemoveRange(userRoles);
        await _dbContext.SaveChangesAsync();

        return await Result<int>.SuccessAsync();
    }

    public async Task<Result<List<Guid>>> GetUserBuildings()
    {
        var tenantId = GetTenantId();
        var buildings = await _unitOfWork.Repository<UserBuilding>()
            .Entities()
            .Where(building => building.UserId == new Guid(_currentUserService.UserId) && building.TenantId == tenantId)
            .Select(x => x.BuildingId)
            .ToListAsync();
        return await Result<List<Guid>>.SuccessAsync(buildings);
    }

    public async Task<Result<List<Guid>>> GetUserSites()
    {
        var tenantId = GetTenantId();
        var sites = await _unitOfWork.Repository<UserSite>()
            .Entities()
            .Where(site => site.UserId == new Guid(_currentUserService.UserId) && site.TenantId == tenantId)
            .Select(x => x.SiteId)
            .ToListAsync();
        return await Result<List<Guid>>.SuccessAsync(sites);
    }

    public async Task<bool> IsUserLoggedInAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return false;
        }

        var user = httpContext.User;
        if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
        {
            return false;
        }

        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return false;
        }

        var dbUser = await _userManager.FindByIdAsync(userId);
        if (dbUser == null || !dbUser.IsActive || dbUser.IsDeleted)
        {
            return false;
        }

        return true;
    }

    private Guid? GetTenantId()
    {
        if (_httpContextAccessor.HttpContext?.Items?.TryGetValue("TenantId", out object tenantIdValue) ?? false)
        {
            if (Guid.TryParse(tenantIdValue?.ToString(), out Guid guidValue))
            {
                return guidValue;
            }
        }

        return null;
    }

    public async Task<string> GenerateJwtAsync()
    {
        var userResponse = await GetAsync(new Guid(_currentUserService.UserId));
        var user = userResponse.Data;

        var userRoles = await GetRolesAsync(user.Id.ToString());
        var roles = userRoles.Data.UserRoles;

        var rolePermissions = new List<Claim>();
        foreach (var role in roles)
            rolePermissions.Add(new Claim(ClaimTypes.Role, role.RoleName));

        var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, user.Id.ToString())
    }.Union(rolePermissions);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: GetSigningCredentials());

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    public string GenerateToken(string email, Guid tenantId)
    {

        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, email),
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: GetSigningCredentials());

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    private SigningCredentials GetSigningCredentials()
    {
        var secret = Encoding.UTF8.GetBytes(_appConfig.Secret);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }

    public async Task<Result<bool>> IsUserDetailAlreadyExist(string email,string phoneNumber)
    {
        bool userWithSameEmail = await _userManager.Users.AnyAsync(x => email != null && x.Email == email && !x.IsDeleted);
        if (userWithSameEmail)
        {
            return await Result<bool>.FailAsync(string.Format(_localizer[MessageConstants.EmailAlreadyUsed], email));
        }
        bool userWithSamePhoneNumber = await _userManager.Users.AnyAsync(x =>phoneNumber !=null && x.PhoneNumber == phoneNumber && !x.IsDeleted);
        if(userWithSamePhoneNumber)
        {
            return await Result<bool>.FailAsync(string.Format(_localizer[MessageConstants.PhoneNumberAlreadyUsed], phoneNumber));
        }
        return await Result<bool>.SuccessAsync(false);
    }

    Task<IResult<Application.Responses.Users.UserResponse>> IUserService.RegisterUser(RegisterRequest request)
    {
        throw new NotImplementedException();
    }

    Task<IResult<Application.Responses.Users.UserResponse>> IUserService.UpdateUserDetail(UpdatePersonalDetailRequest request)
    {
        throw new NotImplementedException();
    }

    Task<Result<List<Application.Responses.Users.UserResponse>>> IUserService.GetAllAsync()
    {
        throw new NotImplementedException();
    }

    Task<IResult<Application.Responses.Users.UserResponse>> IUserService.GetAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<PaginatedResult<Application.Responses.Users.UserResponse>> GetUserByFilter(GridArgument gridArgument)
    {
        throw new NotImplementedException();
    }
}