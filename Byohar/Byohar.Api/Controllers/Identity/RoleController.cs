using Byohar.Api.Attribute;
using Byohar.Application.Interfaces.Identity;
using Byohar.Application.Requests.Identity;
using Byohar.Domain.Entities.Identity;
using Byohar.Shared.Constants.Api;
using Byohar.Shared.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Byohar.Shared.Permission.Permissions;

namespace Byohar.Api.Controllers.Identity
{
    //[Authorize]
    [Route("api/identity/role")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Get All Roles (basic, admin etc.)
        /// </summary>
        /// <returns>Status 200 OK</returns>
        //[Authorize(Policy = Permissions.Roles.View)]
        [HttpPost(ApiConstants.GetAll)]
        public async Task<IActionResult> GetAll(RolePagingRequest request)
        {
            var roles = await _roleService.GetAllAsync(request);
            return Ok(roles);
        }

        /// <summary>
        /// Add a Role
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Status 200 OK</returns>
        [MultiplePermissionAuthorize(Roles.All,Roles.Create)]
        [HttpPost]
        public async Task<IActionResult> Post(RoleRequest request)
        {
            var response = await _roleService.SaveAsync(request);
            return Ok(response);
        }

        /// <summary>
        /// Delete a Role
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        [MultiplePermissionAuthorize(Roles.All,Roles.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _roleService.DeleteAsync(id);
            return Ok(response);
        }

        /// <summary>
        /// Get Permissions By Role Id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns>Status 200 Ok</returns>
        [MultiplePermissionAuthorize(RolePermissions.All,RolePermissions.View)]
        [HttpGet("permissions/{roleId}")]
        public async Task<IActionResult> GetPermissionsByRoleId([FromRoute] Guid roleId)
        {
            var response = await _roleService.GetAllPermissionsAsync(roleId);
            return Ok(response);
        }

        /// <summary>
        /// Edit a Role Claim
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [MultiplePermissionAuthorize(RolePermissions.Edit,RolePermissions.All)]
        [HttpPut("permissions/update")]
        public async Task<IActionResult> Update(PermissionRequest model)
        {
            var response = await _roleService.UpdatePermissionsAsync(model);
            return Ok(response);
        }
        [MultiplePermissionAuthorize(RolePermissions.Create, RolePermissions.All)]
        [HttpPost(ApiConstants.Create)]
        public async Task<IActionResult> Post(CreateUpdateRoleRequest request)
        {
            var response = await _roleService.AddUpdateRolePermission(request);
            return Ok(response);
        }

        [MultiplePermissionAuthorize(RolePermissions.Delete, RolePermissions.All)]
        [HttpPost(ApiConstants.DeleteMany)]
        public async Task<IActionResult> Delete(List<Guid> ids)
        {           
            var response = await _roleService.DeleteMany(ids);
            return Ok(response);
        }

        [MultiplePermissionAuthorize(RolePermissions.Delete, RolePermissions.All)]
        [HttpGet(ApiConstants.DeleteAll)]
        public async Task<IActionResult> DeleteAllRoles()
        {
            var response = await _roleService.DeleteMany(null);
            return Ok(response);
        }

    }
}