namespace EventManagementSystem.Application.Usecases.CreateEvent
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Domain.Models;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using EventManagementSystem.Application.Interfaces;
    using Microsoft.Extensions.Configuration;

    public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, Result<Event>>
    {
        private readonly IRepository<Event> repository;
        private readonly IRepository<EventImage> eventImageRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<CreateEventCommandHandler> logger;
        private readonly IConfiguration configuration;
        private readonly ICurrentUserService currentUserService;
        private readonly IFileStorageService fileStorageService;
        private readonly IAppUserService appUserService;

        public CreateEventCommandHandler(
            IRepository<Event> eventRepository,
            IRepository<EventImage> eventImageRepository,
            IUnitOfWork unitOfWork,
            ILogger<CreateEventCommandHandler> logger,
            IConfiguration configuration,
            ICurrentUserService currentUserService,
            IFileStorageService fileStorageService,
            IAppUserService appUserService)
        {
            this.repository = eventRepository;
            this.eventImageRepository = eventImageRepository;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.configuration = configuration;
            this.currentUserService = currentUserService;
            this.fileStorageService = fileStorageService;
            this.appUserService = appUserService;
        }

        public async Task<Result<Event>> Handle(CreateEventCommand command, CancellationToken cancellationToken = default)
        {
            this.logger.LogInformation("Creating event: {Title}", command.Title);

            // Get current user ID from JWT token
            var currentUserId = this.currentUserService.GetCurrentUserId();
            if (currentUserId == null)
            {
                this.logger.LogWarning("Event creation failed: User not authenticated");
                return Result<Event>.Failure("Event creation failed", null, 401, "User not authenticated.");
            }

            // Verify user is admin
            if (!this.currentUserService.IsCurrentUserAdmin())
            {
                this.logger.LogWarning("Event creation failed: User {UserId} is not an admin", currentUserId);
                return Result<Event>.Failure("Event creation failed", null, 403, "Only admins can create events.");
            }

            // Get admin user details to set AdminName
            var adminUser = await this.appUserService.GetUserAsync(currentUserId.Value.ToString());
            var adminName = adminUser?.UserName ?? string.Empty;

            var newEvent = new Event
            {
                Id = Guid.NewGuid(),
                Title = command.Title,
                Description = command.Description,
                EventDate = command.EventDate,
                Location = command.Location,
                Type = command.Type,
                Capacity = command.Capacity,
                RegistrationCutoffDate = command.RegistrationCutoffDate,
                IsOpenForRegistration = true,
                AdminId = currentUserId.Value, // Set the admin ID from current user
                AdminName = adminName,
            };

            await this.repository.AddAsync(newEvent);
            await this.unitOfWork.SaveChangesAsync(cancellationToken);

            // Get default image URL from file storage service
            string defaultImageUrl;
            try
            {
                defaultImageUrl = this.fileStorageService.GetPublicUrl("default.jpeg");
                this.logger.LogInformation("Retrieved default image URL from storage: {DefaultImageUrl}", defaultImageUrl);
            }
            catch (Exception ex)
            {
                this.logger.LogWarning(ex, "Failed to get default image from storage, falling back to configuration");
                // Fallback to configuration if storage fails
                defaultImageUrl = this.configuration["Default:EventImageURL"] ?? string.Empty;
            }

            // Create default EventImage
            var eventImage = new EventImage
            {
                Id = Guid.NewGuid(),
                EventId = newEvent.Id,
                ImageUrl = defaultImageUrl,
                Event = newEvent,
            };
            await this.eventImageRepository.AddAsync(eventImage);
            await this.unitOfWork.SaveChangesAsync(cancellationToken);

            // Get event with image
            var eventWithImage = await this.repository.GetWithIncludesAsync(newEvent.Id, "Image");

            this.logger.LogInformation("Event created successfully: {EventId} by admin {AdminId}", newEvent.Id, currentUserId);
            return Result<Event>.Success("Event created successfully.", eventWithImage, 201);
        }
    }
}
