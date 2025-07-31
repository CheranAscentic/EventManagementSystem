using EventManagementSystem.API.Interface;
using EventManagementSystem.Application.DTO;
using EventManagementSystem.Application.Usecases.UserLogin;
using EventManagementSystem.Application.Usecases.UserRegistration;
using EventManagementSystem.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementSystem.API.Endpoints
{
    public class UserEndpoint : IEndpointGroup
    {
        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            var auth = app.MapGroup("/api/authentication")
                .WithTags("User Endpoints")
                .WithOpenApi();

            auth.MapPost("/login", HandleUserLogin)
                .WithName("Login")
                .WithSummary("Logs in a User with email and password.")
                .Produces<string>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest);

            auth.MapPost("/Register", HandleUserRegister)
                .WithName("Register")
                .WithSummary("Register a new User with email, password, first name, and last name.")
                .Produces<Result<AppUser>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest);
        }

        private async Task<IResult> HandleUserLogin(
                UserLoginCommand request,
                [FromServices] IMediator mediator,
                [FromServices] ILogger<UserEndpoint> logger)
        {
            logger.LogInformation("Login request received. Email: {request}", request.Email);
            logger.LogDebug("Login request data: {Request}", request);

            var result = await mediator.Send(request);

            logger.LogInformation("Login result. Success: {Success}, Status: {Status}", result.IsSuccess, result.Status);
            logger.LogDebug("Login result: {result}", result);

            return result.ToApiResult();
        }

        private async Task<IResult> HandleUserRegister(
                UserRegistrationCommand request,
                [FromServices] IMediator mediator,
                [FromServices] ILogger<UserEndpoint> logger)
        {
            logger.LogInformation(
                "SignUp request received. Email: {Email}, FirstName: {FirstName}, LastName: {LastName}",
                request.Email,
                request.FirstName,
                request.LastName);
            logger.LogDebug("SignUp request data: {Request}", request);

            var result = await mediator.Send(request);

            logger.LogInformation("SignUp result. Success: {Success}, Status: {Status}", result.IsSuccess, result.Status);
            logger.LogDebug("SignUp result: {result}", result.Value);

            return result.ToApiResult();
        }
    }
}
