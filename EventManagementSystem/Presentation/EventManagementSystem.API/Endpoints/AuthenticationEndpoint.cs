namespace EventManagementSystem.API.Endpoints
{
    using System.Threading.Tasks;
    using EventManagementSystem.API.Interface;
    using EventManagementSystem.API.Services;
    using EventManagementSystem.Application.Usecases.AdminRegistration;
    using EventManagementSystem.Application.Usecases.Login;
    using EventManagementSystem.Application.Usecases.RefreshToken;
    using EventManagementSystem.Application.Usecases.UserRegistration;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    public class AuthenticationEndpoint : IEndpointGroup
    {
        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            var auth = app.MapGroup("/api/authentication")
                .WithTags("Authentication Endpoints")
                .WithOpenApi();

            // Unified login endpoint - Public access
            auth.MapPost("/login", HandleLogin)
                .WithName("Login")
                .WithSummary("Logs in a user (admin or regular user) with email and password.")
                .Produces(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .AllowAnonymous();

            // User registration endpoint - Public access
            auth.MapPost("/register/user", HandleUserRegister)
                .WithName("RegisterUser")
                .WithSummary("Register a new regular user with email, password, first name, and last name.")
                .Produces(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .AllowAnonymous();

            // Admin registration endpoint - Public access (could be restricted in production)
            auth.MapPost("/register/admin", HandleAdminRegister)
                .WithName("RegisterAdmin")
                .WithSummary("Register a new admin user with email, password, first name, last name, and username.")
                .Produces(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .AllowAnonymous();

            auth.MapPost("/refresh", HandleTokenRefresh)
                .WithName("RefreshToken")
                .WithSummary("Given a refresh token, will produce a new Auth Token, RefreshToken")
                .Produces(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .AllowAnonymous();
        }

        private async Task<IResult> HandleLogin(
            LoginCommand request,
            [FromServices] IMediator mediator,
            [FromServices] ILogger<AuthenticationEndpoint> logger,
            [FromServices] MediatorPipelineService pipelineService)
        {
            logger.LogInformation("Login request received. Email: {Email}", request.Email);
            logger.LogDebug("Login request data: {Request}", request);
            return await pipelineService.ExecuteAsync(request, mediator, logger);
        }

        private async Task<IResult> HandleUserRegister(
            UserRegistrationCommand request,
            [FromServices] IMediator mediator,
            [FromServices] ILogger<AuthenticationEndpoint> logger,
            [FromServices] MediatorPipelineService pipelineService)
        {
            logger.LogInformation(
                "User registration request received. Email: {Email}, FirstName: {FirstName}, LastName: {LastName}",
                request.Email,
                request.FirstName,
                request.LastName);
            logger.LogDebug("User registration request data: {Request}", request);
            return await pipelineService.ExecuteAsync(request, mediator, logger);
        }

        private async Task<IResult> HandleAdminRegister(
            AdminRegistrationCommand request,
            [FromServices] IMediator mediator,
            [FromServices] ILogger<AuthenticationEndpoint> logger,
            [FromServices] MediatorPipelineService pipelineService)
        {
            logger.LogInformation(
                "Admin registration request received. Email: {Email}, FirstName: {FirstName}, LastName: {LastName}, UserName: {UserName}",
                request.Email,
                request.FirstName,
                request.LastName,
                request.UserName);
            logger.LogDebug("Admin registration request data: {Request}", request);
            return await pipelineService.ExecuteAsync(request, mediator, logger);
        }

        private async Task<IResult> HandleTokenRefresh(
            RefreshTokenCommand request,
            [FromServices] IMediator mediator,
            [FromServices] ILogger<AuthenticationEndpoint> logger,
            [FromServices] MediatorPipelineService pipelineService)
        {
            logger.LogInformation("Refresh token request received. Token: {Token}", request.RefreshToken);
            logger.LogDebug("Refresh token request data: {Request}", request);
            return await pipelineService.ExecuteAsync(request, mediator, logger);
        }
    }
}
