using DotNetEnv;
using EventManagementSystem.API.Extensions;
using EventManagementSystem.API.Middleware;
using EventManagementSystem.API.Services;
using EventManagementSystem.API.Authorizations;
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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Serilog;

// Load environment variables from .env file
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add environment variables into the Configuration
builder.Configuration.AddEnvironmentVariables();

// Set up Serilog logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information() // Set global minimum level
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddOpenApi();

// Set up Swagger/OpenAPI with JWT authentication
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Event Management System API",
        Version = "v1",
        Description = "A .NET 9 Minimal API for Event Management System",
    });

    // Add JWT authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer",
                },
            },
            Array.Empty<string>()
        },
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

// Add JWT Authentication
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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ClockSkew = TimeSpan.Zero
    };
});

// Add Authorization with custom policies
builder.Services.AddAuthorization(options =>
{
    // Policy that requires the user to be authenticated (any role)
    options.AddPolicy(AuthorizationPolicies.RequireAuthenticatedUser, policy =>
        policy.RequireAuthenticatedUser());

    // Policy that requires the user to have the Admin role
    options.AddPolicy(AuthorizationPolicies.RequireAdminRole, policy =>
        policy.RequireRole("Admin"));

    // Policy that requires the user to have either User or Admin role
    options.AddPolicy(AuthorizationPolicies.RequireUserOrAdminRole, policy =>
        policy.RequireRole("User", "Admin"));
        
    // Policy for resource owner or admin access
    options.AddPolicy(AuthorizationPolicies.RequireResourceOwnerOrAdmin, policy =>
        policy.Requirements.Add(new ResourceOwnerOrAdminRequirement()));

    // Policy for debugging JWT tokens (Development only)
    options.AddPolicy(AuthorizationPolicies.DebugToken, policy =>
        policy.Requirements.Add(new DebugTokenRequirement()));
});

// Register authorization handlers
builder.Services.AddScoped<IAuthorizationHandler, ResourceOwnerOrAdminHandler>();
builder.Services.AddScoped<IAuthorizationHandler, DebugTokenHandler>();

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

// Add HttpContextAccessor for CurrentUserService
builder.Services.AddHttpContextAccessor();

// Register application services
builder.Services.AddScoped<IAppUserService, AppUserService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
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

//// Use HTTPS redirection early (before authentication)
//app.UseHttpsRedirection();

// Use global exception handler
app.UseMiddleware<GlobalExceptionHandler>();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Register all endpoint groups
app.RegisterAllEndpointGroups();

app.Run();