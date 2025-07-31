namespace EventManagementSystem.Application.Usecases.ViewUserEventRegistrations
{
    using System;
    using System.Collections.Generic;
    using MediatR;
    using EventManagementSystem.Domain.Models;
    using EventManagementSystem.Application.DTO;

    public class ViewUserEventRegistrationsQuery : IRequest<Result<List<EventRegistration>>>
    {
        public Guid AppUserId { get; set; }
    }
}
