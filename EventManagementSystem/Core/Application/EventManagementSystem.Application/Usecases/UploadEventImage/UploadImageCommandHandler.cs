namespace EventManagementSystem.Application.Usecases.UploadEventImage
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Application.Interfaces;
    using EventManagementSystem.Domain.Models;
    using MediatR;
    using Microsoft.Extensions.Logging;

    public class UploadImageCommandHandler : IRequestHandler<UploadImageCommand, Result<EventImage>>
    {
        private readonly IRepository<Event> eventRepository;
        private readonly IRepository<EventImage> eventImageRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<UploadImageCommandHandler> logger;

        public UploadImageCommandHandler(
            IRepository<Event> eventRepository,
            IRepository<EventImage> eventImageRepository,
            IUnitOfWork unitOfWork,
            ILogger<UploadImageCommandHandler> logger)
        {
            this.eventRepository = eventRepository;
            this.eventImageRepository = eventImageRepository;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public async Task<Result<EventImage>> Handle(UploadImageCommand command, CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Uploading image for event {EventId}", command.EventId);

            // 1. Get Event
            var eventEntity = await this.eventRepository.GetAsync(command.EventId);
            if (eventEntity == null)
            {
                this.logger.LogWarning("Event not found: {EventId}", command.EventId);
                return Result<EventImage>.Failure("Event not found.", null, 404, "Not Found");
            }

            // 2. Get EventImage (if exists)
            var eventImage = await this.eventImageRepository.GetWithIncludesAsync(command.EventId, "Event");

            if (eventImage != null)
            {
                // Update existing image
                eventImage.ImageUrl = command.ImageUrl;
                await this.eventImageRepository.UpdateAsync(eventImage);
                this.logger.LogInformation("Updated image for event {EventId}", command.EventId);
            }
            else
            {
                // Create new image
                eventImage = new EventImage
                {
                    Id = Guid.NewGuid(),
                    EventId = command.EventId,
                    ImageUrl = command.ImageUrl,
                    Event = eventEntity,
                };
                await this.eventImageRepository.AddAsync(eventImage);
                this.logger.LogInformation("Created new image for event {EventId}", command.EventId);
            }

            await this.unitOfWork.SaveChangesAsync(cancellationToken);
            this.logger.LogInformation("Image upload successful for event {EventId}", command.EventId);
            return Result<EventImage>.Success("Image uploaded successfully.", eventImage, 200);
        }
    }
}
