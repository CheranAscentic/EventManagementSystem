namespace EventManagementSystem.Application.Usecases.CreateEventRegistration
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Domain.Models;
    using EventManagementSystem.Application.Interfaces;

    public class CreateEventRegistrationCommandHandler : IRequestHandler<CreateEventRegistrationCommand, Result<EventRegistration>>
    {
        private readonly IRepository<Event> eventRepository;
        private readonly IRepository<EventRegistration> eventRegistrationRepository;
        private readonly IAppUserService appUserService;
        private readonly ILogger<CreateEventRegistrationCommandHandler> logger;

        public CreateEventRegistrationCommandHandler(
            IRepository<Event> eventRepository,
            IRepository<EventRegistration> eventRegistrationRepository,
            IAppUserService appUserService,
            ILogger<CreateEventRegistrationCommandHandler> logger)
        {
            this.eventRepository = eventRepository;
            this.eventRegistrationRepository = eventRegistrationRepository;
            this.appUserService = appUserService;
            this.logger = logger;
        }

        public async Task<Result<EventRegistration>> Handle(CreateEventRegistrationCommand command, CancellationToken cancellationToken)
        {
            logger.LogInformation("Registering user {UserId} for event {EventId}", command.AppUserId, command.EventId);

            // 1. Get Event
            var eventEntity = await eventRepository.GetAsync(command.EventId);
            if (eventEntity == null)
            {
                logger.LogWarning("Event not found: {EventId}", command.EventId);
                return Result<EventRegistration>.Failure("Event not found.", null, 404, "Not Found");
            }

            // 2. Get User
            var user = await appUserService.GetUserAsync(command.AppUserId.ToString());
            if (user == null)
            {
                logger.LogWarning("User not found: {UserId}", command.AppUserId);
                return Result<EventRegistration>.Failure("User not found.", null, 404, "Not Found");
            }

            // 3. Check if event is open for registration
            if (!eventEntity.IsOpenForRegistration)
            {
                logger.LogWarning("Event is not open for registration: {EventId}", command.EventId);
                return Result<EventRegistration>.Failure("Event is not open for registration.", null, 400, "Registration Closed");
            }

            // 4. Check registration cutoff date
            if (DateTime.Now >= eventEntity.RegistrationCutoffDate)
            {
                logger.LogWarning("Registration cutoff date passed for event: {EventId}", command.EventId);
                return Result<EventRegistration>.Failure("Registration cutoff date has passed.", null, 400, "Registration Closed");
            }

            // 5. Get event with registrations
            eventEntity = await eventRepository.GetWithIncludesAsync(command.EventId, "Registrations");
            if (eventEntity == null)
            {
                logger.LogWarning("Event with registrations not found: {EventId}", command.EventId);
                return Result<EventRegistration>.Failure("Event not found.", null, 404, "Not Found");
            }

            // 6. Check capacity
            if (eventEntity.NoOfRegistrations >= eventEntity.Capacity)
            {
                logger.LogWarning("Event capacity reached: {EventId}", command.EventId);
                return Result<EventRegistration>.Failure("Event capacity reached.", null, 400, "Capacity Full");
            }

            // 7. Check if user already registered
            var alreadyRegistered = eventEntity.Registrations.Any(r => r.UserId == command.AppUserId && !r.IsCanceled);
            if (alreadyRegistered)
            {
                logger.LogWarning("User {UserId} already registered for event {EventId}", command.AppUserId, command.EventId);
                return Result<EventRegistration>.Failure("User already registered for this event.", null, 400, "Already Registered");
            }

            // 8. Create new registration
            var registration = new EventRegistration
            {
                Id = Guid.NewGuid(),
                EventId = command.EventId,
                UserId = command.AppUserId,
                Name = command.Name ?? user.FirstName + " " + user.LastName,
                Email = command.Email ?? user.Email ?? string.Empty,
                Phone = command.PhoneNumber ?? user.PhoneNumber ?? string.Empty,
                RegisteredAt = DateTime.UtcNow,
                IsCanceled = false,
                Event = eventEntity,
                User = user,
            };

            await eventRegistrationRepository.AddAsync(registration);
            logger.LogInformation("User {UserId} registered for event {EventId} successfully", command.AppUserId, command.EventId);
            return Result<EventRegistration>.Success("Registration successful.", registration, 201);
        }
    }
}
