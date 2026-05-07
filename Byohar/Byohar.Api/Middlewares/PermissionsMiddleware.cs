using Byohar.Domain.Entities.Identity;
using Byohar.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Byohar.Api.Middlewares;

public class PermissionsMiddleware
{
    private readonly RequestDelegate _next;

    public PermissionsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        using var scope = context.RequestServices.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var userRoles = context?.User?.FindAll(ClaimTypes.Role).Select(x => x.Value);
        var userId = context?.User?.FindAll(ClaimTypes.NameIdentifier).FirstOrDefault()?.Value;

        if (userRoles != null && userRoles.Any())
        {
            List<RolePermission> rolePermissions = dbContext.RolePermissions.Include(x => x.Role).Where(x => userRoles.Contains(x.Role.Name)).ToList();

            List<UserPermission> userPermissions = dbContext.UserPermissions.Where(x => x.UserId.ToString() == userId).ToList();

            context.User.Identities.FirstOrDefault().AddClaims(userPermissions.Select(x => new Claim(x.ClaimType, x.ClaimValue)));
            context.User.Identities.FirstOrDefault().AddClaims(rolePermissions.Select(x => new Claim(x.ClaimType, x.ClaimValue)));
        }

        await _next(context);
    }

    private static string GetRequiredPermission(HttpContext context)
    {
        // Extract the required permission from the endpoint metadata
        var endpoint = context.GetEndpoint();

        // Check if the endpoint has the [Authorize] attribute
        var authorizeAttribute = endpoint?.Metadata.GetMetadata<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>();

        if (authorizeAttribute != null)
        {
            // Extract the policy from the [Authorize] attribute
            return authorizeAttribute.Policy;
        }

        // Return null if no [Authorize] attribute is found
        return null;
    }
}