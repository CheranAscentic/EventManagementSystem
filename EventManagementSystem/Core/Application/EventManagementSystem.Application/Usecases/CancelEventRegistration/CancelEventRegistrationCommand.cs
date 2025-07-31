namespace EventManagementSystem.Application.Usecases.CancelEventRegistration
{
    using System;
    using MediatR;
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Domain.Models;

    public class CancelEventRegistrationCommand : IRequest<Result<EventRegistration>>
    {
        public Guid AppUserId { get; set; }

        public Guid EventRegistrationId { get; set; }
    }
}
