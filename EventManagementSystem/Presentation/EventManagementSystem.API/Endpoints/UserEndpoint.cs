namespace EventManagementSystem.API.Endpoints
{
    using System.Threading.Tasks;
    using EventManagementSystem.API.Interface;
    using EventManagementSystem.API.Services;
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Application.Usecases.UserRegistration;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    public class UserEndpoint : IEndpointGroup
    {
        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            var users = app.MapGroup("/api/users")
                .WithTags("User Endpoints")
                .WithOpenApi();

            //users.MapPost("/register", HandleUserRegister)
            //    .WithName("UserRegister")
            //    .WithSummary("Register a new User with email, password, first name, and last name.")
            //    .Produces<Result<object>>(StatusCodes.Status200OK)
            //    .ProducesProblem(StatusCodes.Status400BadRequest);
        }

        private async Task<IResult> HandleUserRegister(
            UserRegistrationCommand request,
            [FromServices] IMediator mediator,
            [FromServices] ILogger<UserEndpoint> logger,
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
    }
}
