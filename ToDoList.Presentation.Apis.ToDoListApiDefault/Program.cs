using Microsoft.OpenApi.Models;
using ToDoList.Core.Application;
using ToDoList.Infrastructure.Persistence;
using ToDoList.Presentation.Apis.ToDoListApiDefault.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddPecistenceLayeredRegistration(builder.Configuration);
builder.Services.AddApplicationLayeredRegistration();
builder.Services.AddApiVersioningExtensions();

// CORS - Permitir todas las solicitudes
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
});

var app = builder.Build();

app.UseErrorHandlerMiddleware();

// Enable CORS
app.UseCors("AllowAll");    

// Configure the HTTP request pipeline.
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
