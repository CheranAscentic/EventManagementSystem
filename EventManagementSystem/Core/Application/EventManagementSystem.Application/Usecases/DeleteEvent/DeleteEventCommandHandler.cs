namespace EventManagementSystem.Application.Usecases.DeleteEvent
{
    using MediatR;
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Application.Interfaces;
    using EventManagementSystem.Domain.Models;
    using Microsoft.Extensions.Logging;

    public class DeleteEventCommandHandler : IRequestHandler<DeleteEventCommand, Result<Event>>
    {
        private readonly IRepository<Event> eventRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<DeleteEventCommandHandler> logger;
        private readonly ICurrentUserService currentUserService;

        public DeleteEventCommandHandler(
            IRepository<Event> eventRepository,
            IUnitOfWork unitOfWork,
            ILogger<DeleteEventCommandHandler> logger,
            ICurrentUserService currentUserService)
        {
            this.eventRepository = eventRepository;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.currentUserService = currentUserService;
        }

        public async Task<Result<Event>> Handle(DeleteEventCommand command, CancellationToken cancellationToken = default)
        {
            this.logger.LogInformation("Deleting event: {EventId}", command.Id);

            // Get current user ID from JWT token
            var currentUserId = this.currentUserService.GetCurrentUserId();
            if (currentUserId == null)
            {
                this.logger.LogWarning("Event deletion failed: User not authenticated");
                return Result<Event>.Failure("Event deletion failed", null, 401, "User not authenticated.");
            }

            var eventEntity = await this.eventRepository.GetAsync(command.Id);
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
                this.logger.LogWarning(
                    "Event deletion failed: User {UserId} is not authorized to delete event {EventId}. Event owner: {EventOwnerId}", 
                    currentUserId,
                    command.Id,
                    eventEntity.AdminId);
                return Result<Event>.Failure("Event deletion failed", null, 403, "Only the event owner can delete this event.");
            }

            var removed = await this.eventRepository.RemoveAsync(command.Id);
            if (!removed)
            {
                this.logger.LogWarning("Failed to remove event: {EventId}", command.Id);
                return Result<Event>.Failure("Failed to remove event.", null, 500, "Remove Failed");
            }

            await this.unitOfWork.SaveChangesAsync(cancellationToken);

            this.logger.LogInformation("Event deleted successfully: {EventId} by user {UserId}", eventEntity.Id, currentUserId);
            return Result<Event>.Success("Event deleted successfully.", eventEntity, 200);
        }
    }
}
