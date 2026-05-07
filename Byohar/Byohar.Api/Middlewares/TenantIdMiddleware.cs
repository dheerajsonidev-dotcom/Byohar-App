using Byohar.Persistence.Contexts;
using System.Security.Claims;

namespace Byohar.Api.Middlewares;

public class TenantIdMiddleware
{
    private readonly RequestDelegate _next;

    public TenantIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
    {
       
      
            var userId = context?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var existingUser = dbContext.Users.FirstOrDefault(x => userId != null && x.Id.ToString() == userId);
            if (existingUser != null)
            {
                context.Items["TenantId"] = existingUser.TenantId;
            }
        

        await _next(context);
    }
}