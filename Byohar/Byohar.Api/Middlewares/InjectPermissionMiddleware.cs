using Byohar.Domain.Entities.Identity;
using Byohar.Persistance.Contexts;
using System.Security.Claims;

namespace Byohar.Api.Middlewares;

public class InjectPermissionMiddleware
{
    private readonly RequestDelegate _next;
    private const string EmailHeaderName = "X-Is-From-Email";

    public InjectPermissionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        bool isFromEmail = IsRequestFromEmail(context);

       

        await _next(context);
    }

    private bool IsRequestFromEmail(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(EmailHeaderName, out var headerValue))
        {
            return headerValue.ToString().Equals("true", StringComparison.OrdinalIgnoreCase);
        }
        return false;
    }
}