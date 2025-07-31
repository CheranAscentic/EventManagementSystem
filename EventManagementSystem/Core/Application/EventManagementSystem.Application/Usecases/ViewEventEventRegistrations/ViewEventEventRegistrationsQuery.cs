namespace EventManagementSystem.Application.Usecases.ViewEventEventRegistrations
{
    using System;
    using System.Collections.Generic;
    using MediatR;
    using EventManagementSystem.Domain.Models;
    using EventManagementSystem.Application.DTO;

    public class ViewEventEventRegistrationsQuery : IRequest<Result<List<EventRegistration>>>
    {
        public Guid EventId { get; set; }
    }
}
