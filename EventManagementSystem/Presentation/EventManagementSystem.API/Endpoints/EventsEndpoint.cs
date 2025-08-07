using EventManagementSystem.API.Interface;
using EventManagementSystem.API.Services;
using EventManagementSystem.API.Authorizations;
using EventManagementSystem.Application.Usecases.CreateEvent;
using EventManagementSystem.Application.Usecases.UpdateEvent;
using EventManagementSystem.Application.Usecases.DeleteEvent;
using EventManagementSystem.Application.Usecases.GetEvent;
using EventManagementSystem.Application.Usecases.GetEvents;
using EventManagementSystem.Application.Usecases.GetEventTypes;
using EventManagementSystem.Application.Usecases.GetOwnerEvents;
using EventManagementSystem.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using EventManagementSystem.Application.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using EventManagementSystem.Application.Usecases.GetEventsExtended;

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
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .RequireAuthorization(AuthorizationPolicies.RequireAdminRole);

            events.MapPut("/{id}", HandleUpdateEvent)
                .WithName("UpdateEvent")
                .WithSummary("Update an existing event.")
                .Produces<Result<object>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .RequireAuthorization(AuthorizationPolicies.RequireAdminRole);

            events.MapDelete("/{id}", HandleDeleteEvent)
                .WithName("DeleteEvent")
                .WithSummary("Delete an event.")
                .Produces<Result<object>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .RequireAuthorization(AuthorizationPolicies.RequireAdminRole);

            events.MapGet("/{id}", HandleGetEvent)
                .WithName("GetEvent")
                .WithSummary("Get details of a specific event.")
                .Produces<Result<object>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .AllowAnonymous();

            events.MapGet("/", HandleGetEvents)
                .WithName("GetEvents")
                .WithSummary("Get a list of all events.")
                .Produces<Result<object>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .AllowAnonymous();

            events.MapGet("/owner/{ownerId}", HandleGetOwnerEvents)
                .WithName("GetOwnerEvents")
                .WithSummary("Get all events owned by a specific admin user.")
                .Produces<Result<object>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesProblem(StatusCodes.Status403Forbidden)
                .RequireAuthorization(AuthorizationPolicies.RequireAdminRole);

            events.MapGet("/types", HandleGetEventTypes)
                .WithName("GetEventTypes")
                .WithSummary("Get a list of all supported event types.")
                .Produces<Result<object>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .AllowAnonymous();

            events.MapPost("/GetSorted", HandleGetEventsExtended)
                .WithName("GetEventsExtended")
                .WithSummary("Get a list of events paginated and sorted")
                .Produces<Result<object>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .AllowAnonymous();
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

        private async Task<IResult> HandleGetOwnerEvents(
            [FromServices] IMediator mediator,
            [FromServices] ILogger<EventsEndpoint> logger,
            [FromServices] MediatorPipelineService pipelineService,
            HttpContext httpContext)
        {
            // Extract ownerId from JWT claims
            var ownerIdClaim = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(ownerIdClaim) || !Guid.TryParse(ownerIdClaim, out var ownerId))
            {
                logger.LogWarning("GetOwnerEvents: Could not extract valid ownerId from JWT claims.");
                return Results.Problem("Invalid or missing ownerId in token.", statusCode: StatusCodes.Status401Unauthorized);
            }
            logger.LogInformation("GetOwnerEvents request received. OwnerId: {OwnerId}", ownerId);
            logger.LogDebug("GetOwnerEvents request data: {OwnerId}", ownerId);
            var getRequest = new GetOwnerEventsQuery(ownerId);
            return await pipelineService.ExecuteAsync(getRequest, mediator, logger);
        }

        private async Task<IResult> HandleGetEventTypes(
            [FromServices] IMediator mediator,
            [FromServices] ILogger<EventsEndpoint> logger,
            [FromServices] MediatorPipelineService pipelineService)
        {
            logger.LogInformation("GetEventTypes request received.");
            logger.LogDebug("GetEventTypes request data: none");
            var query = new GetEventTypesQuery();
            return await pipelineService.ExecuteAsync(query, mediator, logger);
        }

        private async Task<IResult> HandleGetEventsExtended(
            [FromBody] GetEventsExtendedQuery request,
            [FromServices] IMediator mediator,
            [FromServices] ILogger<EventsEndpoint> logger,
            [FromServices] MediatorPipelineService pipelineService)
        {
            logger.LogInformation(
                "GetEventsExtended request received. Page: {PageNumber}, ItemsPerPage: {ItemsPerPage}, SearchTerm: {SearchTerm}, EventType: {EventType}",
                request.PageNumber,
                request.ItemsPerPage,
                request.SearchTerm,
                request.EventType);

            logger.LogDebug("GetEventsExtended request data: {Request}", request);
            return await pipelineService.ExecuteAsync(request, mediator, logger);
        }
    }
}
