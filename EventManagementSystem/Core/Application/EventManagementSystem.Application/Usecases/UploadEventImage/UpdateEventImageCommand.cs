namespace EventManagementSystem.Application.Usecases.UploadEventImage
{
    using System;
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Domain.Models;
    using MediatR;

    public class UpdateEventImageCommand : IRequest<Result<EventImage>>
    {
        public Guid EventId { get; set; }

        public string ImageUrl { get; set; } = string.Empty;
    }
}
