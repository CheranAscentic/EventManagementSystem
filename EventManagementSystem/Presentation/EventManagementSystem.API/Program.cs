using DotNetEnv;
using EventManagementSystem.API.Extensions;
using EventManagementSystem.API.Middleware;
using EventManagementSystem.API.Services;
using EventManagementSystem.Application.Behavior;
using EventManagementSystem.Application.Interfaces;
using EventManagementSystem.Application.Usecases.Login;
using EventManagementSystem.Domain.Models;
using EventManagementSystem.Identity.Context;
using EventManagementSystem.Identity.Services;
using EventManagementSystem.Persistence.Context;
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

// Add environment variables into the Configuration
builder.Configuration.AddEnvironmentVariables();

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
        Title = "Event Management System API",
        Version = "v1",
        Description = "A .NET 9 Minimal API for Event Management System",
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
builder.Services.AddDbContext<ApplicationDbContext>(options =>
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
    cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly);
});

// Register application services
builder.Services.AddScoped<IAppUserService, AppUserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<MediatorPipelineService>();

// Register FluentValidation validators
builder.Services.AddValidatorsFromAssemblyContaining<LoginCommandValidator>();

// Register generic repository
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

// Register repositories for Event, EventRegistration, EventImage
builder.Services.AddScoped<IRepository<Event>, GenericRepository<Event>>();
builder.Services.AddScoped<IRepository<EventRegistration>, GenericRepository<EventRegistration>>();
builder.Services.AddScoped<IRepository<EventImage>, GenericRepository<EventImage>>();

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

//foreach (var kvp in app.Configuration.AsEnumerable())
//{
//    Console.WriteLine($"{kvp.Key} = {kvp.Value}");
//}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Event Management System API v1");
        options.RoutePrefix = string.Empty; // Swagger UI at root
    });
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Use global exception handler
app.UseMiddleware<GlobalExceptionHandler>();

// Register all endpoint groups
app.RegisterAllEndpointGroups();

// Enable HTTPS redirection
app.UseHttpsRedirection();

app.Run();