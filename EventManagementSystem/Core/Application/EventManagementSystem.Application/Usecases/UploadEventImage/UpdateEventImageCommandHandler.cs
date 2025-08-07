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

    public class UpdateEventImageCommandHandler : IRequestHandler<UpdateEventImageCommand, Result<EventImage>>
    {
        private readonly IRepository<Event> eventRepository;
        private readonly IRepository<EventImage> eventImageRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<UpdateEventImageCommandHandler> logger;
        private readonly ICurrentUserService currentUserService;

        public UpdateEventImageCommandHandler(
            IRepository<Event> eventRepository,
            IRepository<EventImage> eventImageRepository,
            IUnitOfWork unitOfWork,
            ILogger<UpdateEventImageCommandHandler> logger,
            ICurrentUserService currentUserService)
        {
            this.eventRepository = eventRepository;
            this.eventImageRepository = eventImageRepository;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.currentUserService = currentUserService;
        }

        public async Task<Result<EventImage>> Handle(UpdateEventImageCommand command, CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Uploading image for event {EventId}", command.EventId);

            // Get current user ID from JWT token
            var currentUserId = this.currentUserService.GetCurrentUserId();
            if (currentUserId == null)
            {
                this.logger.LogWarning("Event image update failed: User not authenticated");
                return Result<EventImage>.Failure("Event image update failed", null, 401, "User not authenticated.");
            }

            // 1. Get Event
            var eventEntity = await this.eventRepository.GetAsync(command.EventId);
            if (eventEntity == null)
            {
                this.logger.LogWarning("Event not found: {EventId}", command.EventId);
                return Result<EventImage>.Failure("Event not found.", null, 404, "Not Found");
            }

            // Check if current user is the event owner or is an admin
            var isAdmin = this.currentUserService.IsCurrentUserAdmin();
            var isEventOwner = eventEntity.AdminId == currentUserId;

            if (!isAdmin && !isEventOwner)
            {
                this.logger.LogWarning(
                    "Event image update failed: User {UserId} is not authorized to update image for event {EventId}. Event owner: {EventOwnerId}", 
                    currentUserId,
                    command.EventId,
                    eventEntity.AdminId);
                return Result<EventImage>.Failure("Event image update failed", null, 403, "Only the event owner can update the event image.");
            }

            eventEntity = await this.eventRepository.GetWithIncludesAsync(command.EventId, "Image");
            // 2. Get EventImage (if exists)
            var eventImage = eventEntity!.Image;

            if (eventImage != null)
            {
                // Update existing image
                eventImage.ImageUrl = command.ImageUrl;
                await this.eventImageRepository.UpdateAsync(eventImage);
                this.logger.LogInformation("Updated image for event {EventId} by user {UserId}", command.EventId, currentUserId);
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
                this.logger.LogInformation("Created new image for event {EventId} by user {UserId}", command.EventId, currentUserId);
            }

            await this.unitOfWork.SaveChangesAsync(cancellationToken);
            this.logger.LogInformation("Image upload successful for event {EventId} by user {UserId}", command.EventId, currentUserId);
            return Result<EventImage>.Success("Image uploaded successfully.", eventImage, 200);
        }
    }
}
