using Byohar.Api.Attribute;
using Byohar.Application.Interfaces.Identity;
using Byohar.Application.Requests.Identity;
using Byohar.Application.Responses.Identity;
using Byohar.Application.Validators.Identity;
using Byohar.Shared.Constants.Api;
using Byohar.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Drawing;
using static Byohar.Shared.Permission.Permissions;

namespace Byohar.Api.Controllers.Identity
{
    [Route("api/identity/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Get Users Details
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [MultiplePermissionAuthorize(Users.View, Users.All)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        /// <summary>
        /// Get User By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetAsync(id);
            return Ok(user);
        }

        /// <summary>
        /// Get User View Profile by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        /// [Authorize]
        [HttpGet("GetUserViewProfileById/{id}")]
        public async Task<IActionResult> GetUserViewProfileById(Guid id)
        {
            var user = await _userService.GetUserViewProfile(id);
            return Ok(user);
        }

        /// <summary>
        /// Get User Roles By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        [MultiplePermissionAuthorize(Users.View, Users.All)]
        [HttpGet("roles/{id}")]
        public async Task<IActionResult> GetRolesAsync(string id)
        {
            var userRoles = await _userService.GetRolesAsync(id);
            return Ok(userRoles);
        }

        /// <summary>
        /// Register a User
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Status 200 OK</returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterRequest registerRequest)
        {
            var result = await _userService.RegisterUser(registerRequest);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("getImageToBase64/{_imagePath}")]
        public async Task<IActionResult> ImageToBase64(string _imagePath)
        {
            string _base64String = null;

            _imagePath = "Files/UserProfileImage/" + _imagePath;

            if (_imagePath == "Files/UserProfileImage/null")
            {
                return Ok(await Result<int>.SuccessAsync(_base64String));
            }
            using (Image _image = Image.FromFile(_imagePath))
            {
                using (MemoryStream _mStream = new MemoryStream())
                {
                    _image.Save(_mStream, _image.RawFormat);
                    byte[] _imageBytes = _mStream.ToArray();
                    _base64String = Convert.ToBase64String(_imageBytes);

                    _base64String = "data:image/jpg;base64," + _base64String;
                }
            }

            return Ok(await Result<int>.SuccessAsync(_base64String));
        }

        [HttpPost("update-user")]
        public async Task<IActionResult> UpdateUser(UpdateUserRequest userRequest)
        {
            return Ok(await _userService.UpdateUser(userRequest));
        }

        [HttpPost("personal-details")]
        public async Task<IActionResult> Post()
        {
            UpdatePersonalDetailRequest request = JsonConvert.DeserializeObject<UpdatePersonalDetailRequest>(Request.Form["updateProfile"]);
            var origin = Request.Headers["headers"];
            var profileImage = Request.Form.Files;
            var resp = await _userService.UpdateUserProfileAsync(request, profileImage);
            return Ok(resp);
        }

        /// <summary>
        /// Confirm Email
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <returns>Status 200 OK</returns>
        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmailAsync([FromQuery] Guid userId, [FromQuery] string code)
        {
            return Ok(await _userService.ConfirmEmailAsync(userId, code));
        }

        /// <summary>
        /// Toggle User Status (Activate and Deactivate)
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Status 200 OK</returns>
        [HttpPut("toggle-status")]
        public async Task<IActionResult> ToggleUserStatusAsync(ToggleUserStatusRequest request)
        {
            return Ok(await _userService.ToggleUserStatusAsync(request));
        }

        [HttpPost("Activate")]
        public async Task<IActionResult> UserActivation(UserActivationRequest request)
        {
            return Ok(await _userService.Activate(request));
        }

        [HttpPost("DeActivate")]
        public async Task<IActionResult> UserDeActivation(UserDeActivationRequest request)
        {
            return Ok(await _userService.DeActivate(request));
        }


        /// <summary>
        /// Forgot Password
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Status 200 OK</returns>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var origin = Request.Headers["origin"];
            return Ok(await _userService.ForgotPasswordAsync(request, origin));
        }

        /// <summary>
        /// Reset Password
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Status 200 OK</returns>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordRequest request)
        {
            return Ok(await _userService.ResetPasswordAsync(request));
        }

        /// <summary>
        /// Reset Password
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Status 200 OK</returns>
        [HttpPost("verify-token")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyResetPasswordToken(VerifyResetPasswordTokenRequest request)
        {
            return Ok(await _userService.VerifyResetPasswordToken(request));
        }

        /// <summary>
        /// Export to Excel
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns>Status 200 OK</returns>
        [MultiplePermissionAuthorize(Users.Export, Users.All)]
        [HttpGet("export")]
        public async Task<IActionResult> Export(string searchString = "")
        {
            var data = await _userService.ExportToExcelAsync(searchString);
            return Ok(data);
        }

        [HttpPost("delete-multi-user-byadmin")]
        public async Task<IActionResult> DeleteMultiUser(ActivateDeactivateRequest request)
        {
            return Ok(await _userService.DeleteMultiUser(request));
        }

        [HttpPost("getuserby-filter")]
        public async Task<IActionResult> GetUserByFilter(GridArgument gridArgs)
        {
            var value = await _userService.GetUserByFilter(gridArgs);


            return Ok(value);

        }

        [HttpGet("login-history/{userId}")]
        public async Task<IActionResult> GetLoginHistory(string userId)
        {
            var resp = await _userService.GetLoginHistory(userId);
            return Ok(resp);
        }

        [HttpGet("get-users")]
        public async Task<IActionResult> GetUsers()
        {
            var resp = await _userService.GetUsers();
            return Ok(resp);
        }

        [HttpPut(ApiConstants.Update)]
        public async Task<IActionResult> Update(UpdatePersonalDetailRequest request)
        {
            var result = await _userService.UpdateUserDetail(request);
            return Ok(result);
        }

        [HttpPost("get-list")]
        public async Task<IActionResult> GetUsers(UserPagedRequest request)
        {
            var resp = await _userService.GetUsers(request);
            return Ok(resp);
        }

        [HttpPost("update-user-role")]
        public async Task<IActionResult> UpdateUserRole(UserRoleRequest request)
        {
            var resp = await _userService.UpdateUserRole(request);
            return Ok(resp);
        }

        [HttpPost("get-user-roles")]
        public async Task<IActionResult> GetUserRoles(UserRolePagingRequest request)
        {
            var resp = await _userService.GetUserRolesAsync(request);
            return Ok(resp);
        }

        [HttpPost("user-role-permission")]
        public async Task<IActionResult> UserRolePermission(CreateUpdateUserPermissionRequest request)
        {
            var resp = await _userService.CreateUserPermission(request);
            return Ok(resp);
        }

        [MultiplePermissionAuthorize(Users.Delete, Users.All)]
        [HttpPost(ApiConstants.DeleteMany)]
        public async Task<IActionResult> Delete(List<Guid> ids)
        {
            var response = await _userService.DeleteMany(ids);
            return Ok(response);
        }

        [MultiplePermissionAuthorize(Users.Delete, Users.All)]
        [HttpGet(ApiConstants.DeleteAll)]
        public async Task<IActionResult> DeleteAllUsers()
        {
            var response = await _userService.DeleteMany(null);
            return Ok(response);
        }

        [HttpPost(ApiConstants.User.DeleteRoles)]
        public async Task<IActionResult> DeleteRoles(DeleteUserRoleRequest request)
        {
            var response = await _userService.DeleteUserRoles(request.UserId, request.Roles);
            return Ok(response);
        }

        [HttpDelete(ApiConstants.User.DeleteAllRoles)]
        public async Task<IActionResult> DeleteAllRoles(Guid userId)
        {
            var response = await _userService.DeleteUserRoles(userId, null);
            return Ok(response);
        }
    }
}