using Byohar.Application.Interfaces.Identity;
using Byohar.Application.Interfaces.User;
using Byohar.Application.Requests.Identity;
using Byohar.Application.Validators.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Byohar.Api.Controllers.Identity
{
    [Authorize]
    [Route("api/identity/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAccountService _accountService;
        private readonly ICurrentUserService _currentUser;
        //private readonly IHubContext<SignalRHub> _hubContext;

        public AccountController(IUserService userService, IAccountService accountService, ICurrentUserService currentUser)
        {
            _accountService = accountService;
            _currentUser = currentUser;
            _userService = userService;
        }

        /// <summary>
        /// Update Profile
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Status 200 OK</returns>
        [HttpPut(nameof(UpdateProfile))]
        public async Task<ActionResult> UpdateProfile(UpdateProfileRequest model)
        {
            var response = await _accountService.UpdateProfileAsync(model, new Guid(_currentUser.UserId));
            return Ok(response);
        }

        /// <summary>
        /// Change Password
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Status 200 OK</returns>
        [HttpPut(nameof(ChangePassword))]
        public async Task<ActionResult> ChangePassword(ChangePasswordRequest model)
        {
            var response = await _accountService.ChangePasswordAsync(model, _currentUser.UserId);
            return Ok(response);
        }

        /// <summary>
        /// Get Profile picture by Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Status 200 OK </returns>
        [HttpGet("profile-picture/{userId}")]
        [ResponseCache(NoStore = false, Location = ResponseCacheLocation.Client, Duration = 60)]
        public async Task<IActionResult> GetProfilePictureAsync(string userId)
        {
            return Ok(await _accountService.GetProfilePictureAsync(userId));
        }

        /// <summary>
        /// Update Profile Picture
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Status 200 OK</returns>
        [HttpPost("profile-picture/{userId}")]
        public async Task<IActionResult> UpdateProfilePictureAsync(UpdateProfilePictureRequest request)
        {
            return Ok(await _accountService.UpdateProfilePictureAsync(request, new Guid(_currentUser.UserId)));
        }

        //[HttpPost("savePassword")]
        //[AllowAnonymous]
        //public async Task<IActionResult> SavePassword(TokenRequest request)
        //{
        //    return Ok(await _userService.ChangePassword(request));
        //}

        /// <summary>
        /// logout from other devices for user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Status 200 OK</returns>
        [HttpPost("logoutFromDevice/{userId}")]
        public async Task<ActionResult> LogoutFromDevice(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                //var connections = SignalRHub.Connections.GetConnections(userId);
                //if (connections.Any())
                //    await _hubContext.Clients.Clients(connections).SendAsync(ApplicationConstants.SignalR.DisconnectUser);
                return Ok(new { IsLogout = true });
            }
            else
            {
                return BadRequest();
            }
        }

    }
}