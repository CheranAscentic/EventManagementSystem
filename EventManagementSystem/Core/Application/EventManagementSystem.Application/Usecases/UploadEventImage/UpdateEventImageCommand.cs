namespace EventManagementSystem.Application.Usecases.UploadEventImage
{
    using System;
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Domain.Models;
    using MediatR;
    using Microsoft.AspNetCore.Http;

    public class UpdateEventImageCommand : IRequest<Result<EventImage>>
    {
        public Guid EventId { get; set; }

        public IFormFile ImageFile { get; set; } = null!;
    }
}
