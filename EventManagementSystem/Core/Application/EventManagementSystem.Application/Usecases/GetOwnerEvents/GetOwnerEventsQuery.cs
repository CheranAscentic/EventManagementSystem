namespace EventManagementSystem.Application.Usecases.GetOwnerEvents
{
    using System;
    using System.Collections.Generic;
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Domain.Models;
    using MediatR;

    public class GetOwnerEventsQuery : IRequest<Result<List<Event>>>
    {
        public GetOwnerEventsQuery(Guid ownerId)
        {
            this.OwnerId = ownerId;
        }

        public Guid OwnerId { get; set; }
    }
}