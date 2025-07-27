using ToDoList.Presentation.Apis.ToDoListApiDefault.Middlewares;

namespace ToDoList.Presentation.Apis.ToDoListApiDefault.Extensions
{
    public static class AppEstensions
    {
        public static void UseErrorHandlerMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ErrorHandlerMiddleware>();
        }
    }
}
