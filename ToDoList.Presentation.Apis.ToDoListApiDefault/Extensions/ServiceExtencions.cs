using Asp.Versioning;

namespace ToDoList.Presentation.Apis.ToDoListApiDefault.Extensions
{
    public static class ServiceExtencions
    {
        public static void AddApiVersioningExtensions(this IServiceCollection services)
        {
            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
            });
        }
    }
}
