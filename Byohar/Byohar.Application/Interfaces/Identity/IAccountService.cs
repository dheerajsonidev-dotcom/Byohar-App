using Byohar.Application.Interfaces.Common;
using Byohar.Application.Requests.Identity;
using Byohar.Application.Validators.Identity;
using Byohar.Shared.Wrapper;

namespace Byohar.Application.Interfaces.Identity;

public interface IAccountService : IService
{
    Task<IResult> UpdateProfileAsync(UpdateProfileRequest model, Guid userId);

    Task<IResult> ChangePasswordAsync(ChangePasswordRequest model, string userId);

    Task<IResult<string>> GetProfilePictureAsync(string userId);

    Task<IResult<string>> UpdateProfilePictureAsync(UpdateProfilePictureRequest request, Guid userId);
}