using DotNetEnv;
using EventManagementSystem.API.Extensions;
using EventManagementSystem.API.Middleware;
using EventManagementSystem.Application.Behavior;
using EventManagementSystem.Application.Interfaces;
using EventManagementSystem.Application.Usecases.Authentication.Login;
using EventManagementSystem.Domain.Models;
using EventManagementSystem.Identity.Context;
using EventManagementSystem.Identity.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

//Load in the .env(environment variables). before the buidler
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddOpenApi();

// Add Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Library Management System API",
        Version = "v1",
        Description = "A .NET 9 Minimal API for Library Management System",
    });
});

// Add DbContext for Identity
builder.Services.AddDbContext<IdentityDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("EventManagementSystem.Identity")
    );
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Register IdentityDbContext and Identity
builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 12;
})
.AddEntityFrameworkStores<IdentityDbContext>();

// Add MediatR
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly);
});
// Register AppUserService for IAppUserService
builder.Services.AddScoped<IAppUserService, AppUserService>();
// TokenService
builder.Services.AddScoped<ITokenService, TokenService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
builder.Services.AddValidatorsFromAssemblyContaining<LoginCommand>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "LMS API v1");
        options.RoutePrefix = string.Empty; // Serves Swagger UI at the app's root
    });
    app.MapOpenApi();
}

// Add global exception handler BEFORE other middleware
app.UseMiddleware<GlobalExceptionHandler>();

app.RegisterAllEndpointGroups();

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.Run();