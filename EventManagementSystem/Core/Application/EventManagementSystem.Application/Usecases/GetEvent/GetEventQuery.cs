namespace EventManagementSystem.Application.Usecases.GetEvent
{
    using System;
    using MediatR;
    using EventManagementSystem.Domain.Models;
    using EventManagementSystem.Application.DTO;

    public class GetEventQuery : IRequest<Result<Event>>
    {
        public GetEventQuery(Guid eventId)
        {
            this.EventId = eventId;
        }

        public Guid EventId { get; set; }
    }
}
