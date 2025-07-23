using AppIntgration.Services;

namespace AppIntgration.API.Middleware
{

    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory;

        public AuthenticationMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _scopeFactory = scopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Ignore certain endpoints
            var path = context.Request.Path.Value?.ToLower();
            if (path != null && (
                path.Contains("/swagger") ||
                path.Contains("/api/auth/key") ||
                path.Contains("/api/health")))
            {
                await _next(context);
                return;
            }

            // Check for Authorization header
            if (!context.Request.Headers.ContainsKey("Authorization"))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Authentication key required");
                return;
            }

            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader == null || !authHeader.StartsWith("Bearer "))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid authentication key format");
                return;
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            // Validate key
            using var scope = _scopeFactory.CreateScope();
            var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

            var isValid = await authService.ValidateApiKeyAsync(token);
            if (!isValid)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Authentication key is invalid or expired");
                return;
            }

            await _next(context);
        }
    }
}
