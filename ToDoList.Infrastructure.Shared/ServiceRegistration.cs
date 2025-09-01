using Microsoft.Extensions.DependencyInjection;
using ToDoList.Core.Application.Interfaces;
using ToDoList.Infrastructure.Shared.Services;

namespace ToDoList.Infrastructure.Shared
{
    /// <summary>
    /// Service registration for Infrastructure.Shared layer
    /// </summary>
    public static class ServiceRegistration
    {
        public static void AddSharedLayerRegistration(this IServiceCollection services)
        {
            // Register JWT service
            services.AddScoped<IJwtService, JwtService>();

            // Register password hash service
            services.AddScoped<IPasswordHashService, PasswordHashService>();

            // Register auth service
            services.AddScoped<IAuthService, AuthService>();
        }
    }
}
