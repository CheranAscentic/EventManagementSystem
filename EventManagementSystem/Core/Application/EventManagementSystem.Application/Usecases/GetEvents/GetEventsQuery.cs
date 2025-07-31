namespace EventManagementSystem.Application.Usecases.GetEvents
{
    using MediatR;
    using EventManagementSystem.Domain.Models;
    using EventManagementSystem.Application.DTO;
    using System.Collections.Generic;

    public class GetEventsQuery : IRequest<Result<List<Event>>>
    {
        // No properties needed for fetching all events
    }
}
