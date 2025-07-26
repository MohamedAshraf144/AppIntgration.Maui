using AppIntgration.Services;

namespace AppIntgration.API.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAuthService _authService; // 🔧 Inject directly instead of using scope factory

        public AuthenticationMiddleware(RequestDelegate next, IAuthService authService)
        {
            _next = next;
            _authService = authService; // ✅ Direct injection = same singleton instance
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 🔓 Skip authentication for these endpoints
            var path = context.Request.Path.Value?.ToLower();
            if (path != null && ShouldSkipAuthentication(path))
            {
                await _next(context);
                return;
            }

            // ✅ Check for Authorization header
            if (!context.Request.Headers.ContainsKey("Authorization"))
            {
                await WriteUnauthorizedResponse(context, "Authentication key required");
                return;
            }

            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader == null || !authHeader.StartsWith("Bearer "))
            {
                await WriteUnauthorizedResponse(context, "Invalid authentication key format. Use: Bearer YOUR_KEY");
                return;
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            // 🔍 Validate key using the same singleton instance
            var isValid = await _authService.ValidateApiKeyAsync(token);
            if (!isValid)
            {
                await WriteUnauthorizedResponse(context, "Authentication key is invalid or expired");
                return;
            }

            await _next(context);
        }

        /// <summary>
        /// Determine which endpoints should skip authentication
        /// </summary>
        private static bool ShouldSkipAuthentication(string path)
        {
            var allowedPaths = new[]
            {
                // Swagger و API Documentation
                "/swagger",
                "/swagger/",
                "/swagger/index.html",
                "/swagger/v1/swagger.json",
                
                // API Key generation and validation
                "/api/auth",
                
                // Health checks
                "/api/health",
                "/health",
                
                // WeatherForecast (for testing)
                "/weatherforecast",
                
                // Static files
                "/favicon.ico",
                "/css/",
                "/js/",
                "/images/"
            };

            return allowedPaths.Any(allowedPath =>
                path.StartsWith(allowedPath, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Send 401 response with clear message
        /// </summary>
        private static async Task WriteUnauthorizedResponse(HttpContext context, string message)
        {
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";

            var response = new
            {
                success = false,
                message = message,
                timestamp = DateTime.UtcNow,
                path = context.Request.Path.Value,
                method = context.Request.Method,
                hint = "Get API key from: GET /api/auth/apikey, then use: Authorization: Bearer YOUR_KEY"
            };

            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
        }
    }
}