namespace EventManagementSystem.Application.Usecases.CancelUserEventRegistration
{
    using System;
    using MediatR;
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Domain.Models;

    public class CancelUserEventRegistrationCommand : IRequest<Result<EventRegistration>>
    {
        public Guid AppUserId { get; set; }

        public Guid EventRegistrationId { get; set; }
    }
}
