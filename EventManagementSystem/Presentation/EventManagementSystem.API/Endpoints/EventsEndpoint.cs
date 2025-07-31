using EventManagementSystem.API.Interface;
using EventManagementSystem.Application.Usecases.CreateEvent;
using EventManagementSystem.Application.Usecases.UpdateEvent;
using EventManagementSystem.Application.Usecases.DeleteEvent;
using EventManagementSystem.Application.Usecases.GetEvent;
using EventManagementSystem.Application.Usecases.GetEvents;
using EventManagementSystem.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using EventManagementSystem.Application.DTO;
using System.Collections.Generic;

namespace EventManagementSystem.API.Endpoints
{
    public class EventsEndpoint : IEndpointGroup
    {
        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            var events = app.MapGroup("/api/events")
                .WithTags("Events Endpoints")
                .WithOpenApi();

            events.MapPost("/", HandleCreateEvent)
                .WithName("CreateEvent")
                .WithSummary("Create a new event.")
                .Produces<Result<Event>>(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status400BadRequest);

            events.MapPut("/{id}", HandleUpdateEvent)
                .WithName("UpdateEvent")
                .WithSummary("Update an existing event.")
                .Produces<Result<Event>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest);

            events.MapDelete("/{id}", HandleDeleteEvent)
                .WithName("DeleteEvent")
                .WithSummary("Delete an event.")
                .Produces<Result<Event>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound);

            events.MapGet("/{id}", HandleGetEvent)
                .WithName("GetEvent")
                .WithSummary("Get details of a specific event.")
                .Produces<Result<Event>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound);

            events.MapGet("/", HandleGetEvents)
                .WithName("GetEvents")
                .WithSummary("Get a list of all events.")
                .Produces<Result<List<Event>>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest);
        }

        private async Task<IResult> HandleCreateEvent(
            [FromBody] CreateEventCommand request,
            [FromServices] IMediator mediator,
            [FromServices] ILogger<EventsEndpoint> logger)
        {
            logger.LogInformation("CreateEvent request received. Title: {Title}", request.Title);
            var result = await mediator.Send(request);
            logger.LogInformation("CreateEvent result. Success: {Success}, Status: {Status}", result.IsSuccess, result.Status);
            return result.ToApiResult();
        }

        private async Task<IResult> HandleUpdateEvent(
            Guid id,
            [FromBody] UpdateEventCommand request,
            [FromServices] IMediator mediator,
            [FromServices] ILogger<EventsEndpoint> logger)
        {
            logger.LogInformation("UpdateEvent request received. Id: {Id}", id);
            var updateRequest = new UpdateEventCommand(
                id,
                request.Title,
                request.Description,
                request.EventDate,
                request.Location,
                request.Type,
                request.Capacity,
                request.RegistrationCutoffDate
            );
            var result = await mediator.Send(updateRequest);
            logger.LogInformation("UpdateEvent result. Success: {Success}, Status: {Status}", result.IsSuccess, result.Status);
            return result.ToApiResult();
        }

        private async Task<IResult> HandleDeleteEvent(
            Guid id,
            [FromServices] IMediator mediator,
            [FromServices] ILogger<EventsEndpoint> logger)
        {
            logger.LogInformation("DeleteEvent request received. Id: {Id}", id);
            var result = await mediator.Send(new DeleteEventCommand(id));
            logger.LogInformation("DeleteEvent result. Success: {Success}, Status: {Status}", result.IsSuccess, result.Status);
            return result.ToApiResult();
        }

        private async Task<IResult> HandleGetEvent(
            Guid id,
            [FromServices] IMediator mediator,
            [FromServices] ILogger<EventsEndpoint> logger)
        {
            logger.LogInformation("GetEvent request received. Id: {Id}", id);
            var result = await mediator.Send(new GetEventQuery(id));
            logger.LogInformation("GetEvent result. Success: {Success}, Status: {Status}", result.IsSuccess, result.Status);
            return result.ToApiResult();
        }

        private async Task<IResult> HandleGetEvents(
            [FromServices] IMediator mediator,
            [FromServices] ILogger<EventsEndpoint> logger)
        {
            logger.LogInformation("GetEvents request received.");
            var result = await mediator.Send(new GetEventsQuery());
            logger.LogInformation("GetEvents result. Success: {Success}, Status: {Status}", result.IsSuccess, result.Status);
            return result.ToApiResult();
        }
    }
}
