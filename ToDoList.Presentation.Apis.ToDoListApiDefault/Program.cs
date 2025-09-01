using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using ToDoList.Core.Application;
using ToDoList.Infrastructure.Persistence;
using ToDoList.Infrastructure.Shared;
using ToDoList.Presentation.Apis.ToDoListApiDefault.Extensions;
using ToDoList.Presentation.Apis.ToDoListApiDefault.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Layer registrations
builder.Services.AddPecistenceLayeredRegistration(builder.Configuration);
builder.Services.AddApplicationLayeredRegistration();
builder.Services.AddSharedLayerRegistration(); // Add JWT and Auth services
builder.Services.AddApiVersioningExtensions();

builder.Services.AddMemoryCache();

// JWT Configuration
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"] ?? throw new ArgumentNullException("Jwt:SecretKey is required");
var issuer = jwtSettings["Issuer"] ?? "ToDoListApi";
var audience = jwtSettings["Audience"] ?? "ToDoListApp";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine($"Token validated for user: {context.Principal?.Identity?.Name}");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

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
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ToDoList API",
        Version = "v1",
        Description = "A comprehensive API for task management with JWT authentication"
    });

    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    // Keep existing idempotency key configuration
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
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDoList API V1");
        c.DefaultModelsExpandDepth(-1); // Hide schemas section
    });
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Authentication and Authorization middleware (order is important)
app.UseAuthentication();
app.UseJwtMiddleware(); // Custom JWT middleware for additional processing
app.UseAuthorization();

app.MapControllers();

app.Run();
