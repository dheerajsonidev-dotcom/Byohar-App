using Byohar.Api.Attribute;
using Byohar.Application.Interfaces.Identity;
using Byohar.Application.Requests.Identity;
using Microsoft.AspNetCore.Mvc;
using static Byohar.Shared.Permission.Permissions;

namespace Byohar.Api.Controllers.Identity
{
    [Route("api/identity/RolePermission")]
    [ApiController]
    public class RolePermissionController : ControllerBase
    {
        private readonly IRolePermissionService _RolePermissionService;

        public RolePermissionController(IRolePermissionService RolePermissionService)
        {
            _RolePermissionService = RolePermissionService;
        }

        /// <summary>
        /// Get All Role Claims(e.g. Product Create Permission)
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [MultiplePermissionAuthorize(RolePermissions.View, RolePermissions.All)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var RolePermissions = await _RolePermissionService.GetAllAsync();
            return Ok(RolePermissions);
        }

        /// <summary>
        /// Get All Role Claims By Id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns>Status 200 OK</returns>
        [MultiplePermissionAuthorize(RolePermissions.View, RolePermissions.All)]
        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetAllByRoleId(Guid roleId)
        {
            var response = await _RolePermissionService.GetAllByRoleIdAsync(roleId);
            return Ok(response);
        }

        /// <summary>
        /// Add a Role Claim
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Status 200 OK </returns>
        [MultiplePermissionAuthorize(RolePermissions.Create, RolePermissions.All)]
        [HttpPost]
        public async Task<IActionResult> Post(RolePermissionRequest request)
        {
            var response = await _RolePermissionService.SaveAsync(request);
            return Ok(response);
        }

        /// <summary>
        /// Delete a Role Claim
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        [MultiplePermissionAuthorize(RolePermissions.Delete, RolePermissions.All)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _RolePermissionService.DeleteAsync(id);
            return Ok(response);
        }

        /// <summary>
        /// Get User and Role permissions 
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Status 200 OK</returns>

        [HttpPost("user-role-permissions")]
        public async Task<IActionResult> GetUserRolePermissions(UserRolePermissionRequest request)
        {
            var response = await _RolePermissionService.GetPermissionsByRoleIdsAndUserId(request);
            return Ok(response);
        }

        [HttpGet("user-permissions/{id}")]
        public async Task<IActionResult> GetPermissionsByModule(string id)
        {
            var response = await _RolePermissionService.GetPermissions(id);
            return Ok(response);
        }
    }
}