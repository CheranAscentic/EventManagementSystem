namespace EventManagementSystem.Application.Usecases.MakeUserEventRegistration
{
    using System;
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Domain.Models;
    using MediatR;

    public class MakeUserEventRegistrationCommand : IRequest<Result<EventRegistration>>
    {
        public Guid AppUserId { get; set; }

        public Guid EventId { get; set; }

        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }
    }
}
