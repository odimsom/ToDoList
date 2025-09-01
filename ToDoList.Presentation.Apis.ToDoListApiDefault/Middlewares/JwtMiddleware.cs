using System.Security.Claims;
using ToDoList.Core.Application.Interfaces;

namespace ToDoList.Presentation.Apis.ToDoListApiDefault.Middlewares
{
    /// <summary>
    /// Middleware for JWT token validation and user context setting
    /// </summary>
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtMiddleware> _logger;

        public JwtMiddleware(RequestDelegate next, ILogger<JwtMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IJwtService jwtService)
        {
            var token = context.Request.Headers["Authorization"]
                .FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var userId = jwtService.ValidateToken(token);
                    if (userId.HasValue)
                    {
                        // Token is valid - user context is already set by JWT authentication handler
                        _logger.LogDebug("Valid JWT token for user {UserId}", userId.Value);
                    }
                    else
                    {
                        _logger.LogWarning("Invalid JWT token provided");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error validating JWT token");
                }
            }

            await _next(context);
        }
    }

    /// <summary>
    /// Extension method for registering JWT middleware
    /// </summary>
    public static class JwtMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwtMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtMiddleware>();
        }
    }
}
