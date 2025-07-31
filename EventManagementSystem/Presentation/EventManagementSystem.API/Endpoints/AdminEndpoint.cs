namespace EventManagementSystem.API.Endpoints
{
    using EventManagementSystem.API.Interface;
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Application.Usecases.AdminLogin;
    using EventManagementSystem.Application.Usecases.AdminRegistration;
    using EventManagementSystem.Domain.Models;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    public class AdminEndpoint : IEndpointGroup
    {
        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            var admin = app.MapGroup("/api/admin")
                .WithTags("Admin Endpoints")
                .WithOpenApi();

            admin.MapPost("/login", HandleAdminLogin)
                .WithName("AdminLogin")
                .WithSummary("Logs in an Admin with email and password.")
                .Produces<Result<LoginDTO>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest);

            admin.MapPost("/register", HandleAdminRegister)
                .WithName("AdminRegister")
                .WithSummary("Register a new Admin with email, password, first name, last name, and username.")
                .Produces<Result<AppUser>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest);
        }

        private async Task<IResult> HandleAdminLogin(
            AdminLoginCommand request,
            [FromServices] IMediator mediator,
            [FromServices] ILogger<AdminEndpoint> logger)
        {
            logger.LogInformation("Admin login request received. Email: {Email}", request.Email);
            logger.LogDebug("Admin login request data: {Request}", request);

            var result = await mediator.Send(request);

            logger.LogInformation("Admin login result. Success: {Success}, Status: {Status}", result.IsSuccess, result.Status);
            logger.LogDebug("Admin login result: {Result}", result);

            return result.ToApiResult();
        }

        private async Task<IResult> HandleAdminRegister(
            AdminRegistrationCommand request,
            [FromServices] IMediator mediator,
            [FromServices] ILogger<AdminEndpoint> logger)
        {
            logger.LogInformation(
                "Admin registration request received. Email: {Email}, FirstName: {FirstName}, LastName: {LastName}, UserName: {UserName}",
                request.Email,
                request.FirstName,
                request.LastName,
                request.UserName);
            logger.LogDebug("Admin registration request data: {Request}", request);

            var result = await mediator.Send(request);

            logger.LogInformation("Admin registration result. Success: {Success}, Status: {Status}", result.IsSuccess, result.Status);
            logger.LogDebug("Admin registration result: {Result}", result.Value);

            return result.ToApiResult();
        }
    }
}
