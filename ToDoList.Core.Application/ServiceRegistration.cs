using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using ToDoList.Core.Application.Behaviours;
using ToDoList.Core.Application.Interfaces;
using ToDoList.Core.Application.Services;

namespace ToDoList.Core.Application
{
    public static class ServiceRegistration
    {
        public static void AddApplicationLayeredRegistration(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddTransient<IQueueTaskItemService, QueueTaskItemService>();
        }
    }
}
