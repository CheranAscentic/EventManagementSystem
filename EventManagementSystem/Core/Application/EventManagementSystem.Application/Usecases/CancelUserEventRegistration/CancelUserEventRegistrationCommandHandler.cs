namespace EventManagementSystem.Application.Usecases.CancelUserEventRegistration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Domain.Models;
    using EventManagementSystem.Application.Interfaces;

    public class CancelUserEventRegistrationCommandHandler : IRequestHandler<CancelUserEventRegistrationCommand, Result<EventRegistration>>
    {
        private readonly IRepository<Event> eventRepository;
        private readonly IRepository<EventRegistration> eventRegistrationRepository;
        private readonly IAppUserService appUserService;
        private readonly ILogger<CancelUserEventRegistrationCommandHandler> logger;
        private readonly IUnitOfWork unitOfWork;

        public CancelUserEventRegistrationCommandHandler(
            IRepository<Event> eventRepository,
            IRepository<EventRegistration> eventRegistrationRepository,
            IAppUserService appUserService,
            IUnitOfWork unitOfWork,
            ILogger<CancelUserEventRegistrationCommandHandler> logger)
        {
            this.eventRepository = eventRepository;
            this.eventRegistrationRepository = eventRegistrationRepository;
            this.appUserService = appUserService;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public async Task<Result<EventRegistration>> Handle(CancelUserEventRegistrationCommand command, CancellationToken cancellationToken)
        {
            logger.LogInformation("Canceling event registration {EventRegistrationId} for user {AppUserId}", command.EventRegistrationId, command.AppUserId);

            // 1. Get AppUser
            var user = await appUserService.GetUserAsync(command.AppUserId.ToString());
            if (user == null)
            {
                logger.LogWarning("User not found: {AppUserId}", command.AppUserId);
                return Result<EventRegistration>.Failure("User not found.", null, 404, "Not Found");
            }

            // 2. Get EventRegistration
            var registration = await eventRegistrationRepository.GetAsync(command.EventRegistrationId);
            if (registration == null)
            {
                logger.LogWarning("Event registration not found: {EventRegistrationId}", command.EventRegistrationId);
                return Result<EventRegistration>.Failure("Event registration not found.", null, 404, "Not Found");
            }

            // 3. Get Event
            var eventEntity = await eventRepository.GetAsync(registration.EventId);
            if (eventEntity == null)
            {
                logger.LogWarning("Event not found for registration: {EventId}", registration.EventId);
                return Result<EventRegistration>.Failure("Event not found for registration.", null, 404, "Not Found");
            }

            // 4. Remove EventRegistration
            var removed = await eventRegistrationRepository.RemoveAsync(command.EventRegistrationId);
            if (!removed)
            {
                logger.LogWarning("Failed to remove event registration: {EventRegistrationId}", command.EventRegistrationId);
                return Result<EventRegistration>.Failure("Failed to remove event registration.", null, 500, "Remove Failed");
            }

            // 5. Save changes
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Event registration {EventRegistrationId} canceled successfully for user {AppUserId}", command.EventRegistrationId, command.AppUserId);
            return Result<EventRegistration>.Success("Event registration canceled successfully.", registration, 200);
        }
    }
}
