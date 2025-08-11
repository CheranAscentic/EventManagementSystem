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
        private readonly IFileStorageService fileStorageService;


        public UpdateEventImageCommandHandler(
            IRepository<Event> eventRepository,
            IRepository<EventImage> eventImageRepository,
            IUnitOfWork unitOfWork,
            ILogger<UpdateEventImageCommandHandler> logger,
            ICurrentUserService currentUserService,
            IFileStorageService fileStorageService)
        {
            this.eventRepository = eventRepository;
            this.eventImageRepository = eventImageRepository;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.currentUserService = currentUserService;
            this.fileStorageService = fileStorageService;
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

            try
            {
                // Upload image to storage service
                string imageUrl;
                using (var imageStream = command.ImageFile.OpenReadStream())
                {
                    this.logger.LogInformation("Uploading image to storage for event {EventId}", command.EventId);
                    imageUrl = await this.fileStorageService.UploadEventImageAsync(
                        imageStream,
                        command.ImageFile.FileName,
                        command.EventId);
                }

                this.logger.LogInformation("Image uploaded successfully to storage. URL: {ImageUrl}", imageUrl);

                // Get event with image relationship
                eventEntity = await this.eventRepository.GetWithIncludesAsync(command.EventId, "Image");

                // 2. Get EventImage (if exists)
                var eventImage = eventEntity!.Image;

                if (eventImage != null)
                {
                    // Update existing image
                    var oldImageUrl = eventImage.ImageUrl;
                    eventImage.ImageUrl = imageUrl;
                    await this.eventImageRepository.UpdateAsync(eventImage);
                    this.logger.LogInformation("Updated existing image for event {EventId} by user {UserId}. Old URL: {OldUrl}, New URL: {NewUrl}",
                        command.EventId, currentUserId, oldImageUrl, imageUrl);
                }
                else
                {
                    // Create new image
                    eventImage = new EventImage
                    {
                        Id = Guid.NewGuid(),
                        EventId = command.EventId,
                        ImageUrl = imageUrl,
                        Event = eventEntity,
                    };
                    await this.eventImageRepository.AddAsync(eventImage);
                    this.logger.LogInformation("Created new image for event {EventId} by user {UserId}. URL: {ImageUrl}",
                        command.EventId, currentUserId, imageUrl);
                }

                await this.unitOfWork.SaveChangesAsync(cancellationToken);
                this.logger.LogInformation("Image upload and database update completed successfully for event {EventId} by user {UserId}",
                    command.EventId, currentUserId);

                return Result<EventImage>.Success("Image uploaded successfully.", eventImage, 200);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to upload image for event {EventId} by user {UserId}. Error: {Error}",
                    command.EventId, currentUserId, ex.Message);
                return Result<EventImage>.Failure("Image upload failed", null, 500, "An error occurred while uploading the image. Please try again.");
            }
        }
    }
}
