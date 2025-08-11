namespace EventManagementSystem.Application.Usecases.UpdateUserEventRegistration
{
    using System;
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Domain.Models;
    using MediatR;

    public class UpdateUserEventRegistrationCommand : IRequest<Result<EventRegistration>>
    {
        public Guid EventRegistrationId { get; set; }

        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }
    }
}