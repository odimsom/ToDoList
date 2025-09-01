using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using ToDoList.Core.Domain.RepositoriesInterfaces;
using ToDoList.Core.Domain.RepositoriesInterfaces.Common;
using ToDoList.Core.Domain.Shared;
using ToDoList.Infrastructure.Persistence.Context;
using ToDoList.Infrastructure.Persistence.Repositories;

namespace ToDoList.Infrastructure.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPecistenceLayeredRegistration(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");
            services.AddDbContext<ToDoListContext>(opt =>
            {
                opt.EnableSensitiveDataLogging();
                opt.UseSqlServer(connectionString,
                    m => m.MigrationsAssembly(typeof(ToDoListContext).Assembly.FullName));
            }, ServiceLifetime.Scoped, ServiceLifetime.Scoped);

            services.AddStereotype(typeof(IGenericRepository<,>), Assembly.GetExecutingAssembly());

            // Register specific repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        }
    }
}
