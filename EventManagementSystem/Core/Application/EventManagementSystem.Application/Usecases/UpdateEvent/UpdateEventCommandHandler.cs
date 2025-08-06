namespace EventManagementSystem.Application.Usecases.UpdateEvent
{
    using MediatR;
    using Microsoft.Extensions.Logging;
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Domain.Models;
    using EventManagementSystem.Application.Interfaces;

    public class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand, Result<Event>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<UpdateEventCommandHandler> logger;
        private readonly IRepository<Event> repository;
        private readonly ICurrentUserService currentUserService;

        public UpdateEventCommandHandler(
            IRepository<Event> repository, 
            IUnitOfWork unitOfWork, 
            ILogger<UpdateEventCommandHandler> logger,
            ICurrentUserService currentUserService)
        {
            this.repository = repository;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.currentUserService = currentUserService;
        }

        public async Task<Result<Event>> Handle(UpdateEventCommand command, CancellationToken cancellationToken = default)
        {
            this.logger.LogInformation("Updating event: {EventId}", command.Id);

            // Get current user ID from JWT token
            var currentUserId = this.currentUserService.GetCurrentUserId();
            if (currentUserId == null)
            {
                this.logger.LogWarning("Event update failed: User not authenticated");
                return Result<Event>.Failure("Event update failed", null, 401, "User not authenticated.");
            }

            // Get the event from the database
            var eventEntity = await this.repository.GetAsync(command.Id);
            if (eventEntity == null)
            {
                this.logger.LogWarning("Event not found: {EventId}", command.Id);
                return Result<Event>.Failure("Event not found.", null, 404, "Not Found");
            }

            // Check if current user is the event owner or is an admin
            var isAdmin = this.currentUserService.IsCurrentUserAdmin();
            var isEventOwner = eventEntity.AdminId == currentUserId;

            if (!isAdmin && !isEventOwner)
            {
                this.logger.LogWarning("Event update failed: User {UserId} is not authorized to update event {EventId}. Event owner: {EventOwnerId}", 
                    currentUserId, command.Id, eventEntity.AdminId);
                return Result<Event>.Failure("Event update failed", null, 403, "Only the event owner can update this event.");
            }

            // Update only provided fields
            if (!string.IsNullOrWhiteSpace(command.Title))
            {
                eventEntity.Title = command.Title;
            }

            if (!string.IsNullOrWhiteSpace(command.Description))
            {
                eventEntity.Description = command.Description;
            }

            if (command.EventDate != null)
            {
                eventEntity.EventDate = command.EventDate.Value;
            }

            if (!string.IsNullOrWhiteSpace(command.Location))
            {
                eventEntity.Location = command.Location;
            }

            if (!string.IsNullOrWhiteSpace(command.Type))
            {
                eventEntity.Type = command.Type;
            }

            if (command.Capacity != null)
            {
                eventEntity.Capacity = command.Capacity.Value;
            }

            if (command.RegistrationCutoffDate != null)
            {
                eventEntity.RegistrationCutoffDate = command.RegistrationCutoffDate.Value;
            }

            await this.unitOfWork.SaveChangesAsync(cancellationToken);

            this.logger.LogInformation("Event updated successfully: {EventId} by user {UserId}", eventEntity.Id, currentUserId);
            return Result<Event>.Success("Event updated successfully.", eventEntity, 200);
        }
    }
}
