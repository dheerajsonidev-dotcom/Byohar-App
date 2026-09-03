namespace Byohar.Api.Middlewares
{
    public class CustomHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Set security and custom headers for the response
            context.Response.OnStarting(() =>
            {
                // Security headers
                //context.Response.Headers["Content-Security-Policy"] =
                //    "default-src 'self'; script-src 'self'; style-src 'self'; img-src 'self' data:; frame-src 'self'; upgrade-insecure-requests;"; // for restricting all script, style from any source


                // Set Content Security Policy to restrict script sources
                context.Response.Headers.Add("Content-Security-Policy",
                    "default-src 'self'; " +
                    "script-src 'self' https://maps.googleapis.com https://cdnjs.cloudflare.com https://cdn.jsdelivr.net 'unsafe-inline' 'unsafe-eval'; " +
                    "style-src 'self' https://fonts.googleapis.com 'unsafe-inline'; " +
                    "font-src 'self' https://fonts.gstatic.com data:; " +
                    "img-src 'self' data:; " +
                    "connect-src 'self' https://maps.googleapis.com; " +
                    "frame-src 'self' javascript:;");

                // context.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains; preload"; // use in production only
                context.Response.Headers["X-Content-Type-Options"] = "nosniff";
                context.Response.Headers["X-Frame-Options"] = "DENY";
                context.Response.Headers["Referrer-Policy"] = "no-referrer";
                context.Response.Headers["Permissions-Policy"] = "geolocation=(self), microphone=()";

                // Conditionally set Content-Type header
                if (context.Request.Headers["Accept"].ToString().Contains("application/json"))
                {
                    context.Response.Headers["Content-Type"] = "application/json";
                }

                return Task.CompletedTask;
            });

            // Continue to the next middleware in the pipeline
            await _next(context);
        }
    }
}