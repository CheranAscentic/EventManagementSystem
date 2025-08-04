using EventManagementSystem.API.Interface;
using EventManagementSystem.API.Services;
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
using System.Threading.Tasks;

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
                .Produces<Result<object>>(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status400BadRequest);

            events.MapPut("/{id}", HandleUpdateEvent)
                .WithName("UpdateEvent")
                .WithSummary("Update an existing event.")
                .Produces<Result<object>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest);

            events.MapDelete("/{id}", HandleDeleteEvent)
                .WithName("DeleteEvent")
                .WithSummary("Delete an event.")
                .Produces<Result<object>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound);

            events.MapGet("/{id}", HandleGetEvent)
                .WithName("GetEvent")
                .WithSummary("Get details of a specific event.")
                .Produces<Result<object>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound);

            events.MapGet("/", HandleGetEvents)
                .WithName("GetEvents")
                .WithSummary("Get a list of all events.")
                .Produces<Result<object>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest);
        }

        private async Task<IResult> HandleCreateEvent(
            [FromBody] CreateEventCommand request,
            [FromServices] IMediator mediator,
            [FromServices] ILogger<EventsEndpoint> logger,
            [FromServices] MediatorPipelineService pipelineService)
        {
            logger.LogInformation("CreateEvent request received. Title: {Title}", request.Title);
            logger.LogDebug("CreateEvent request data: {Request}", request);
            return await pipelineService.ExecuteAsync(request, mediator, logger);
        }

        private async Task<IResult> HandleUpdateEvent(
            Guid id,
            [FromBody] UpdateEventCommand request,
            [FromServices] IMediator mediator,
            [FromServices] ILogger<EventsEndpoint> logger,
            [FromServices] MediatorPipelineService pipelineService)
        {
            logger.LogInformation("UpdateEvent request received. Id: {Id}", id);
            logger.LogDebug("UpdateEvent request data: {Request}", request);
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
            return await pipelineService.ExecuteAsync(updateRequest, mediator, logger);
        }

        private async Task<IResult> HandleDeleteEvent(
            Guid id,
            [FromServices] IMediator mediator,
            [FromServices] ILogger<EventsEndpoint> logger,
            [FromServices] MediatorPipelineService pipelineService)
        {
            logger.LogInformation("DeleteEvent request received. Id: {Id}", id);
            logger.LogDebug("DeleteEvent request data: {Id}", id);
            var deleteRequest = new DeleteEventCommand(id);
            return await pipelineService.ExecuteAsync(deleteRequest, mediator, logger);
        }

        private async Task<IResult> HandleGetEvent(
            Guid id,
            [FromServices] IMediator mediator,
            [FromServices] ILogger<EventsEndpoint> logger,
            [FromServices] MediatorPipelineService pipelineService)
        {
            logger.LogInformation("GetEvent request received. Id: {Id}", id);
            logger.LogDebug("GetEvent request data: {Id}", id);
            var getRequest = new GetEventQuery(id);
            return await pipelineService.ExecuteAsync(getRequest, mediator, logger);
        }

        private async Task<IResult> HandleGetEvents(
            [FromServices] IMediator mediator,
            [FromServices] ILogger<EventsEndpoint> logger,
            [FromServices] MediatorPipelineService pipelineService)
        {
            logger.LogInformation("GetEvents request received.");
            logger.LogDebug("GetEvents request data: none");
            var getRequest = new GetEventsQuery();
            return await pipelineService.ExecuteAsync(getRequest, mediator, logger);
        }
    }
}
