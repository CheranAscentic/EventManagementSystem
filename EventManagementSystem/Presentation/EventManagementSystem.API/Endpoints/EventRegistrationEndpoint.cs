namespace EventManagementSystem.API.Endpoints
{
    using System.Collections.Generic;
    using EventManagementSystem.API.Interface;
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
                .Produces<Result<EventRegistration>>(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status400BadRequest);

            registrations.MapDelete("/{id}", HandleCancelEventRegistration)
                .WithName("CancelEventRegistration")
                .WithSummary("Cancel an event registration.")
                .Produces<Result<EventRegistration>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound);

            registrations.MapGet("/user/{userId}", HandleViewUserEventRegistrations)
                .WithName("ViewUserEventRegistrations")
                .WithSummary("Get all event registrations for a user.")
                .Produces<Result<List<EventRegistration>>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound);

            registrations.MapGet("/event/{eventId}", HandleViewEventEventRegistrations)
                .WithName("ViewEventEventRegistrations")
                .WithSummary("Get all registrations for an event.")
                .Produces<Result<List<EventRegistration>>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound);

            registrations.MapPost("/event-image", HandleUploadEventImage)
                .WithName("UploadEventImage")
                .WithSummary("Upload an image for an event.")
                .Produces<Result<EventImage>>(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status400BadRequest);
        }

        private async Task<IResult> HandleCreateEventRegistration(
            [FromBody] MakeUserEventRegistrationCommand request,
            [FromServices] IMediator mediator,
            [FromServices] ILogger<EventRegistrationEndpoint> logger)
        {
            logger.LogInformation("CreateEventRegistration request received. EventId: {EventId}, AppUserId: {AppUserId}", request.EventId, request.AppUserId);
            var result = await mediator.Send(request);
            logger.LogInformation("CreateEventRegistration result. Success: {Success}, Status: {Status}", result.IsSuccess, result.Status);
            return result.ToApiResult();
        }

        private async Task<IResult> HandleCancelEventRegistration(
            Guid id,
            [FromBody] CancelUserEventRegistrationCommand request,
            [FromServices] IMediator mediator,
            [FromServices] ILogger<EventRegistrationEndpoint> logger)
        {
            logger.LogInformation("CancelEventRegistration request received. EventRegistrationId: {Id}, AppUserId: {AppUserId}", id, request.AppUserId);
            var cancelRequest = new CancelUserEventRegistrationCommand
            {
                EventRegistrationId = id,
                AppUserId = request.AppUserId,
            };
            var result = await mediator.Send(cancelRequest);
            logger.LogInformation("CancelEventRegistration result. Success: {Success}, Status: {Status}", result.IsSuccess, result.Status);
            return result.ToApiResult();
        }

        private async Task<IResult> HandleViewUserEventRegistrations(
            Guid userId,
            [FromServices] IMediator mediator,
            [FromServices] ILogger<EventRegistrationEndpoint> logger)
        {
            logger.LogInformation("ViewUserEventRegistrations request received. AppUserId: {AppUserId}", userId);
            var result = await mediator.Send(new ViewUserEventRegistrationsQuery { AppUserId = userId });
            logger.LogInformation("ViewUserEventRegistrations result. Success: {Success}, Status: {Status}", result.IsSuccess, result.Status);
            return result.ToApiResult();
        }

        private async Task<IResult> HandleViewEventEventRegistrations(
            Guid eventId,
            [FromServices] IMediator mediator,
            [FromServices] ILogger<EventRegistrationEndpoint> logger)
        {
            logger.LogInformation("ViewEventEventRegistrations request received. EventId: {EventId}", eventId);
            var result = await mediator.Send(new ViewEventEventRegistrationsQuery { EventId = eventId });
            logger.LogInformation("ViewEventEventRegistrations result. Success: {Success}, Status: {Status}", result.IsSuccess, result.Status);
            return result.ToApiResult();
        }

        private async Task<IResult> HandleUploadEventImage(
            [FromBody] UpdateEventImageCommand request,
            [FromServices] IMediator mediator,
            [FromServices] ILogger<EventRegistrationEndpoint> logger)
        {
            logger.LogInformation("UploadEventImage request received. EventId: {EventId}, ImageUrl: {ImageUrl}", request.EventId, request.ImageUrl);
            var result = await mediator.Send(request);
            logger.LogInformation("UploadEventImage result. Success: {Success}, Status: {Status}", result.IsSuccess, result.Status);
            return result.ToApiResult();
        }
    }
}
