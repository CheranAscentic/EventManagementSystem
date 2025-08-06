namespace EventManagementSystem.API.Endpoints
{
    using System.Collections.Generic;
    using EventManagementSystem.API.Interface;
    using EventManagementSystem.API.Services;
    using EventManagementSystem.API.Authorizations;
    using EventManagementSystem.Application.Usecases.ViewUserEventRegistrations;
    using EventManagementSystem.Application.Usecases.ViewEventEventRegistrations;
    using EventManagementSystem.Application.Usecases.UploadEventImage;
    using EventManagementSystem.Domain.Models;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Application.Usecases.MakeUserEventRegistration;
    using EventManagementSystem.Application.Usecases.CancelUserEventRegistration;
    using System.Threading.Tasks;

    public class EventRegistrationEndpoint : IEndpointGroup
    {
        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            var registrations = app.MapGroup("/api/event-registrations")
                .WithTags("Event Registration Endpoints")
                .WithOpenApi();

            registrations.MapPost("/", HandleCreateEventRegistration)
                .WithName("CreateEventRegistration")
                .WithSummary("Register a user for an event.")
                .Produces<Result<object>>(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .RequireAuthorization(AuthorizationPolicies.RequireUserOrAdminRole);

            registrations.MapDelete("/{id}", HandleCancelEventRegistration)
                .WithName("CancelEventRegistration")
                .WithSummary("Cancel an event registration.")
                .Produces<Result<object>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .RequireAuthorization(AuthorizationPolicies.RequireUserOrAdminRole);

            registrations.MapGet("/user/{userId}", HandleViewUserEventRegistrations)
                .WithName("ViewUserEventRegistrations")
                .WithSummary("Get all event registrations for a user.")
                .Produces<Result<object>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .RequireAuthorization(AuthorizationPolicies.RequireUserOrAdminRole);

            registrations.MapGet("/event/{eventId}", HandleViewEventEventRegistrations)
                .WithName("ViewEventEventRegistrations")
                .WithSummary("Get all registrations for an event.")
                .Produces<Result<object>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .RequireAuthorization(AuthorizationPolicies.RequireAdminRole);

            registrations.MapPost("/event-image", HandleUploadEventImage)
                .WithName("UploadEventImage")
                .WithSummary("Upload an image for an event.")
                .Produces<Result<object>>(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .RequireAuthorization(AuthorizationPolicies.RequireAdminRole);
        }

        private async Task<IResult> HandleCreateEventRegistration(
            [FromBody] MakeUserEventRegistrationCommand request,
            [FromServices] IMediator mediator,
            [FromServices] ILogger<EventRegistrationEndpoint> logger,
            [FromServices] MediatorPipelineService pipelineService)
        {
            logger.LogInformation("CreateEventRegistration request received. EventId: {EventId}, AppUserId: {AppUserId}", request.EventId, request.AppUserId);
            logger.LogDebug("CreateEventRegistration request data: {Request}", request);
            return await pipelineService.ExecuteAsync(request, mediator, logger);
        }

        private async Task<IResult> HandleCancelEventRegistration(
            Guid id,
            [FromBody] CancelUserEventRegistrationCommand request,
            [FromServices] IMediator mediator,
            [FromServices] ILogger<EventRegistrationEndpoint> logger,
            [FromServices] MediatorPipelineService pipelineService)
        {
            logger.LogInformation("CancelEventRegistration request received. EventRegistrationId: {Id}, AppUserId: {AppUserId}", id, request.AppUserId);
            logger.LogDebug("CancelEventRegistration request data: {Request}", request);
            var cancelRequest = new CancelUserEventRegistrationCommand
            {
                EventRegistrationId = id,
                AppUserId = request.AppUserId,
            };
            return await pipelineService.ExecuteAsync(cancelRequest, mediator, logger);
        }

        private async Task<IResult> HandleViewUserEventRegistrations(
            Guid userId,
            [FromServices] IMediator mediator,
            [FromServices] ILogger<EventRegistrationEndpoint> logger,
            [FromServices] MediatorPipelineService pipelineService,
            HttpContext httpContext)
        {
            logger.LogInformation("ViewUserEventRegistrations request received. AppUserId: {AppUserId}", userId);
            logger.LogDebug("ViewUserEventRegistrations request data: {UserId}", userId);

            // Check if the current user is accessing their own data or is an admin
            var currentUserId = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = httpContext.User.IsInRole("Admin");

            if (!isAdmin && (string.IsNullOrEmpty(currentUserId) || !Guid.TryParse(currentUserId, out var currentUserGuid) || currentUserGuid != userId))
            {
                logger.LogWarning("Unauthorized access attempt to user registrations. Current user: {CurrentUserId}, Requested user: {RequestedUserId}", currentUserId, userId);
                return Results.Forbid();
            }

            var query = new ViewUserEventRegistrationsQuery { AppUserId = userId };
            return await pipelineService.ExecuteAsync(query, mediator, logger);
        }

        private async Task<IResult> HandleViewEventEventRegistrations(
            Guid eventId,
            [FromServices] IMediator mediator,
            [FromServices] ILogger<EventRegistrationEndpoint> logger,
            [FromServices] MediatorPipelineService pipelineService)
        {
            logger.LogInformation("ViewEventEventRegistrations request received. EventId: {EventId}", eventId);
            logger.LogDebug("ViewEventEventRegistrations request data: {EventId}", eventId);
            var query = new ViewEventEventRegistrationsQuery { EventId = eventId };
            return await pipelineService.ExecuteAsync(query, mediator, logger);
        }

        private async Task<IResult> HandleUploadEventImage(
            [FromBody] UpdateEventImageCommand request,
            [FromServices] IMediator mediator,
            [FromServices] ILogger<EventRegistrationEndpoint> logger,
            [FromServices] MediatorPipelineService pipelineService)
        {
            logger.LogInformation("UploadEventImage request received. EventId: {EventId}, ImageUrl: {ImageUrl}", request.EventId, request.ImageUrl);
            logger.LogDebug("UploadEventImage request data: {Request}", request);
            return await pipelineService.ExecuteAsync(request, mediator, logger);
        }
    }
}
