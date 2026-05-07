namespace Byohar.Api.Middlewares;

public class TimeZoneMiddleware
{
    private readonly RequestDelegate _next;

    public TimeZoneMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("Timezone-Offset", out var timeZone))
        {
            context.Items["TimezoneOffset"] = timeZone.ToString();
        }

        await _next(context);
    }
}