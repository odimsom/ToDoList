using Microsoft.OpenApi.Models;
using ToDoList.Core.Application;
using ToDoList.Infrastructure.Persistence;
using ToDoList.Presentation.Apis.ToDoListApiDefault.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddPecistenceLayeredRegistration(builder.Configuration);
builder.Services.AddApplicationLayeredRegistration();
builder.Services.AddApiVersioningExtensions();

builder.Services.AddMemoryCache();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mi API", Version = "v1" });
    
    c.AddSecurityDefinition("IdempotencyKey", new OpenApiSecurityScheme
    {
        Description = "Idempotency key for safe retries",
        Name = "Idempotency-Key",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "IdempotencyKey"
    });
});

var app = builder.Build();

app.UseErrorHandlerMiddleware();

app.UseCors("AllowAll");    

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
