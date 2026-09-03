using Byohar.Application.Interfaces.Identity;
using Byohar.Application.Requests.Identity;
using Byohar.Application.Validators.Identity;
using Byohar.Domain.Entities.Identity;
using Byohar.Shared.Wrapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using MessageConstants = Byohar.Shared.Constants.User.MessageConstants;

namespace Byohar.Infrastructure.Services.Identity;

public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IStringLocalizer<AccountService> _localizer;

    public AccountService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IStringLocalizer<AccountService> localizer)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _localizer = localizer;
    }

    public async Task<IResult> ChangePasswordAsync(ChangePasswordRequest model, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return await Result.SuccessAsync(_localizer[MessageConstants.PasswordChangeInitiated]);
        }

        var identityResult = await _userManager.ChangePasswordAsync(
            user,
            model.Password,
            model.NewPassword);

        var errors = identityResult.Errors.Select(e => _localizer[e.Description].ToString()).ToList();

        return identityResult.Succeeded ? await Result.SuccessAsync() : await Result.FailAsync(errors);
    }

    public async Task<IResult> UpdateProfileAsync(UpdateProfileRequest request, Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            return await Result.SuccessAsync(_localizer[MessageConstants.ProfileUpdateInitiated]);
        }

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            var userWithSamePhoneNumber = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber && x.Id != userId);

            if (userWithSamePhoneNumber != null)
            {
                return await Result.FailAsync(string.Format(_localizer[MessageConstants.PhoneNumberAlreadyUsed], request.PhoneNumber));
            }
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.PhoneNumber;

        var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

        if (request.PhoneNumber != phoneNumber)
        {
            var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, request.PhoneNumber);
        }

        var identityResult = await _userManager.UpdateAsync(user);

        var errors = identityResult.Errors.Select(e => _localizer[e.Description].ToString()).ToList();

        await _signInManager.RefreshSignInAsync(user);

        return identityResult.Succeeded ? await Result.SuccessAsync() : await Result.FailAsync(errors);
    }

    public async Task<IResult<string>> GetProfilePictureAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return await Result<string>.SuccessAsync(string.Empty);
        }

        return await Result<string>.SuccessAsync(user.ProfilePictureDataUrl);
    }

    public async Task<IResult<string>> UpdateProfilePictureAsync(UpdateProfilePictureRequest request, Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            return await Result<string>.SuccessAsync(_localizer[MessageConstants.ProfileUpdateInitiated]);
        }

        // var filePath = _uploadService.UploadAsync(request);
        var filePath = string.Empty; // TODO: Implement file upload logic
        user.ProfilePictureDataUrl = filePath;

        var identityResult = await _userManager.UpdateAsync(user);

        var errors = identityResult.Errors.Select(e => _localizer[e.Description].ToString()).ToList();
        return identityResult.Succeeded ? await Result<string>.SuccessAsync(data: filePath) : await Result<string>.FailAsync(errors);
    }
}