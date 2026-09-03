using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Byohar.Shared.Permission;

namespace Byohar.Api.Attribute
{
    public class MultiplePermissionAuthorize : AuthorizeAttribute
    {
        private readonly string[] _permissions;
        
        public MultiplePermissionAuthorize(params string[] permissions)
        {
            _permissions = permissions;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {

            if (!_permissions.Any(permission =>
                context.HttpContext.User.HasClaim(c => (c.Type == ApplicationClaimTypes.Permission) && c.Value == permission)))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}