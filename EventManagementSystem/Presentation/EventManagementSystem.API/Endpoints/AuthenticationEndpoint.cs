using EventManagementSystem.API.Interface;
using EventManagementSystem.Application.DTO;
using EventManagementSystem.Application.Usecases.Authentication.Login;
using EventManagementSystem.Application.Usecases.Authentication.SignUp;
using EventManagementSystem.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementSystem.API.Endpoints
{
    public class AuthenticationEndpoint : IEndpointGroup
    {
        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            var auth = app.MapGroup("/api/authentication")
                .WithTags("Authentication")
                .WithOpenApi();

            auth.MapPost("/login", HandleLogin)
                .WithName("Login")
                .WithSummary("Logs in a user with email and password.")
                .Produces<string>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest);

            auth.MapPost("/signup", HandleSignUp)
                .WithName("SignUp")
                .WithSummary("Signs up a new user with email, password, first name, and last name.")
                .Produces<StandardResponseObject<AppUser>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest);
        }


        private async Task<IResult> HandleLogin(
                StandardRequestObject<LoginCommand> request,
                [FromServices] IMediator mediator,
                [FromServices] ILogger<AuthenticationEndpoint> logger)
        {
            logger.LogInformation("Login request received. Email: {Email}", request.Data?.Email);

            var response = await mediator.Send(request.Data);

            logger.LogInformation("Login response. Success: {Success}, Status: {Status}", response.Success, response.Status);

            return response.ToApiResult();
        }

        private async Task<IResult> HandleSignUp(
                StandardRequestObject<SignUpCommand> request,
                [FromServices] IMediator mediator,
                [FromServices] ILogger<AuthenticationEndpoint> logger)
        {
            logger.LogInformation(
                "SignUp request received. Email: {Email}, FirstName: {FirstName}, LastName: {LastName}",
                request.Data?.Email,
                request.Data?.FirstName,
                request.Data?.LastName);

            var response = await mediator.Send(request.Data);

            logger.LogInformation("SignUp response. Success: {Success}, Status: {Status}", response.Success, response.Status);

            return response.ToApiResult();
        }
    }
}
