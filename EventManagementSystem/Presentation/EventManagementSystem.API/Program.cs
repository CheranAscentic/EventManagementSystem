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
using Serilog;

//Load in the .env(environment variables). before the buidler
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

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

// Ensure you have the FluentValidation.DependencyInjectionExtensions package installed
// You can install it via NuGet Package Manager with the following command:
// Install-Package FluentValidation.DependencyInjectionExtensions

builder.Services.AddValidatorsFromAssemblyContaining<LoginCommandValidator>();

// Add pipeline behavior for validation
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// Add this before builder.Build()
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        policy
            .AllowAnyOrigin()      // Or specify origins with .WithOrigins("https://example.com")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Place this before endpoint registration and after middleware
app.UseCors("DefaultCorsPolicy");

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