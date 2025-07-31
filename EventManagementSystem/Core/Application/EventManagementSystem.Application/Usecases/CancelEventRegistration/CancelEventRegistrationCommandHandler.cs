namespace EventManagementSystem.Application.Usecases.CancelEventRegistration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Domain.Models;
    using EventManagementSystem.Application.Interfaces;

    public class CancelEventRegistrationCommandHandler : IRequestHandler<CancelEventRegistrationCommand, Result<EventRegistration>>
    {
        private readonly IRepository<Event> eventRepository;
        private readonly IRepository<EventRegistration> eventRegistrationRepository;
        private readonly IAppUserService appUserService;
        private readonly ILogger<CancelEventRegistrationCommandHandler> logger;
        private readonly IUnitOfWork unitOfWork;

        public CancelEventRegistrationCommandHandler(
            IRepository<Event> eventRepository,
            IRepository<EventRegistration> eventRegistrationRepository,
            IAppUserService appUserService,
            IUnitOfWork unitOfWork,
            ILogger<CancelEventRegistrationCommandHandler> logger)
        {
            this.eventRepository = eventRepository;
            this.eventRegistrationRepository = eventRegistrationRepository;
            this.appUserService = appUserService;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public async Task<Result<EventRegistration>> Handle(CancelEventRegistrationCommand command, CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Canceling event registration {EventRegistrationId} for user {AppUserId}", command.EventRegistrationId, command.AppUserId);

            // 1. Get AppUser
            var user = await this.appUserService.GetUserAsync(command.AppUserId.ToString());
            if (user == null)
            {
                this.logger.LogWarning("User not found: {AppUserId}", command.AppUserId);
                return Result<EventRegistration>.Failure("User not found.", null, 404, "Not Found");
            }

            // 2. Get EventRegistration
            var registration = await this.eventRegistrationRepository.GetAsync(command.EventRegistrationId);
            if (registration == null)
            {
                this.logger.LogWarning("Event registration not found: {EventRegistrationId}", command.EventRegistrationId);
                return Result<EventRegistration>.Failure("Event registration not found.", null, 404, "Not Found");
            }

            // 3. Get Event
            var eventEntity = await this.eventRepository.GetAsync(registration.EventId);
            if (eventEntity == null)
            {
                this.logger.LogWarning("Event not found for registration: {EventId}", registration.EventId);
                return Result<EventRegistration>.Failure("Event not found for registration.", null, 404, "Not Found");
            }

            // 4. Remove EventRegistration
            var removed = await this.eventRegistrationRepository.RemoveAsync(command.EventRegistrationId);
            if (!removed)
            {
                this.logger.LogWarning("Failed to remove event registration: {EventRegistrationId}", command.EventRegistrationId);
                return Result<EventRegistration>.Failure("Failed to remove event registration.", null, 500, "Remove Failed");
            }

            // 5. Save changes
            await this.unitOfWork.SaveChangesAsync(cancellationToken);
            this.logger.LogInformation("Event registration {EventRegistrationId} canceled successfully for user {AppUserId}", command.EventRegistrationId, command.AppUserId);
            return Result<EventRegistration>.Success("Event registration canceled successfully.", registration, 200);
        }
    }
}
