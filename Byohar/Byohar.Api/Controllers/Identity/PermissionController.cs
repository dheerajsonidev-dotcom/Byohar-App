using Byohar.Application.Interfaces.Identity;
using Byohar.Application.Requests.Identity;
using Byohar.Shared.Constants.Api;
using Microsoft.AspNetCore.Mvc;

namespace Byohar.Api.Controllers.Identity
{
    public class PermissionController : BaseApiController<PermissionController>
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }   
        
        [HttpPost(ApiConstants.GetAll)]
        public async Task<IActionResult> GetAll(PermissionPagingRequest request)
        {
            var permissions = await _permissionService.GetAllAsync(request);
            return Ok(permissions);
        }


    }
}
