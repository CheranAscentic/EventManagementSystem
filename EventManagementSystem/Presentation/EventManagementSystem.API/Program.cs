using DotNetEnv;
using EventManagementSystem.API.Extensions;
using EventManagementSystem.API.Middleware;
using EventManagementSystem.Application.Behavior;
using EventManagementSystem.Application.Interfaces;
using EventManagementSystem.Application.Usecases.UserLogin;
using EventManagementSystem.Domain.Models;
using EventManagementSystem.Identity.Context;
using EventManagementSystem.Identity.Services;
using EventManagementSystem.Persistence.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

// Load environment variables from .env file
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Set up Serilog logging
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddOpenApi();

// Set up Swagger/OpenAPI
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

// Set up database context for Identity
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

// Set up Identity
builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 12;
})
.AddEntityFrameworkStores<IdentityDbContext>();

// Set up database context for ApplicationDbContext
builder.Services.AddDbContext<EventManagementSystem.Persistence.Context.ApplicationDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("EventManagementSystem.Persistence")
    );
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Set up MediatR and validation pipeline
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(UserLoginCommand).Assembly);
});

// Register application services
builder.Services.AddScoped<IAppUserService, AppUserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register FluentValidation validators
builder.Services.AddValidatorsFromAssemblyContaining<UserLoginCommandValidator>();

// Register generic repository
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
// Register repositories for Event, EventRegistration, EventImage
builder.Services.AddScoped<IRepository<EventManagementSystem.Domain.Models.Event>, GenericRepository<EventManagementSystem.Domain.Models.Event>>();
builder.Services.AddScoped<IRepository<EventManagementSystem.Domain.Models.EventRegistration>, GenericRepository<EventManagementSystem.Domain.Models.EventRegistration>>();
builder.Services.AddScoped<IRepository<EventManagementSystem.Domain.Models.EventImage>, GenericRepository<EventManagementSystem.Domain.Models.EventImage>>();

// Set up CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Enable CORS
app.UseCors("DefaultCorsPolicy");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "LMS API v1");
        options.RoutePrefix = string.Empty; // Swagger UI at root
    });
    app.MapOpenApi();
}

// Use global exception handler
app.UseMiddleware<GlobalExceptionHandler>();

// Register all endpoint groups
app.RegisterAllEndpointGroups();

// Enable HTTPS redirection
app.UseHttpsRedirection();

app.Run();