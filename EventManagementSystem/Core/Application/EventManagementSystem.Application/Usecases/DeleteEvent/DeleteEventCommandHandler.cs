namespace EventManagementSystem.Application.Usecases.DeleteEvent
{
    using MediatR;
    using Microsoft.Extensions.Logging;
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Domain.Models;
    using EventManagementSystem.Application.Interfaces;

    public class DeleteEventCommandHandler : IRequestHandler<DeleteEventCommand, Result<Event>>
    {
        private readonly IRepository<Event> eventRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<DeleteEventCommandHandler> logger;

        public DeleteEventCommandHandler(IRepository<Event> eventRepository, IUnitOfWork unitOfWork, ILogger<DeleteEventCommandHandler> logger)
        {
            this.eventRepository = eventRepository;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public async Task<Result<Event>> Handle(DeleteEventCommand command, CancellationToken cancellationToken = default)
        {
            this.logger.LogInformation("Deleting event: {EventId}", command.Id);

            var eventEntity = await this.eventRepository.GetAsync(command.Id);
            if (eventEntity == null)
            {
                this.logger.LogWarning("Event not found: {EventId}", command.Id);
                return Result<Event>.Failure("Event not found.", null, 404, "Not Found");
            }

            var removed = await this.eventRepository.RemoveAsync(command.Id);
            if (!removed)
            {
                this.logger.LogWarning("Failed to remove event: {EventId}", command.Id);
                return Result<Event>.Failure("Failed to remove event.", null, 500, "Remove Failed");
            }

            await this.unitOfWork.SaveChangesAsync(cancellationToken);

            this.logger.LogInformation("Event deleted successfully: {EventId}", eventEntity.Id);
            return Result<Event>.Success("Event deleted successfully.", eventEntity, 200);
        }
    }
}
