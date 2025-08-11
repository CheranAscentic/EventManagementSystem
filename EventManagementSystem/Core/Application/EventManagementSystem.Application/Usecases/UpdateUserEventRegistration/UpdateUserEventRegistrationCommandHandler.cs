namespace EventManagementSystem.Application.Usecases.UpdateUserEventRegistration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Application.Interfaces;
    using EventManagementSystem.Domain.Models;
    using MediatR;
    using Microsoft.Extensions.Logging;

    public class UpdateUserEventRegistrationCommandHandler : IRequestHandler<UpdateUserEventRegistrationCommand, Result<EventRegistration>>
    {
        private readonly IRepository<EventRegistration> eventRegistrationRepository;
        private readonly IRepository<Event> eventRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<UpdateUserEventRegistrationCommandHandler> logger;

        public UpdateUserEventRegistrationCommandHandler(
            IRepository<EventRegistration> eventRegistrationRepository,
            IRepository<Event> eventRepository,
            IUnitOfWork unitOfWork,
            ILogger<UpdateUserEventRegistrationCommandHandler> logger)
        {
            this.eventRegistrationRepository = eventRegistrationRepository;
            this.eventRepository = eventRepository;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public async Task<Result<EventRegistration>> Handle(UpdateUserEventRegistrationCommand command, CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Updating event registration {EventRegistrationId}", command.EventRegistrationId);

            // 1. Get EventRegistration
            var registration = await this.eventRegistrationRepository.GetAsync(command.EventRegistrationId);
            if (registration == null)
            {
                this.logger.LogWarning("EventRegistration not found: {EventRegistrationId}", command.EventRegistrationId);
                return Result<EventRegistration>.Failure("Event registration not found.", null, 404, "Not Found");
            }

            // 2. Get Event
            var eventEntity = await this.eventRepository.GetWithIncludesAsync(registration.EventId, "Registrations");
            if (eventEntity == null)
            {
                this.logger.LogWarning("Event not found for registration: {EventId}", registration.EventId);
                return Result<EventRegistration>.Failure("Event not found.", null, 404, "Not Found");
            }

            // 3. Check if event is open for registration
            if (!eventEntity.IsOpenForRegistration)
            {
                this.logger.LogWarning("Event is not open for registration: {EventId}", registration.EventId);
                return Result<EventRegistration>.Failure("Event is not open for registration.", null, 400, "Registration Closed");
            }

            // 4. Check registration cutoff date
            if (DateTime.Now >= eventEntity.RegistrationCutoffDate)
            {
                this.logger.LogWarning("Registration cutoff date passed for event: {EventId}", registration.EventId);
                return Result<EventRegistration>.Failure("Registration cutoff date has passed.", null, 400, "Registration Closed");
            }

            // 5. Update registration fields
            registration.Name = command.Name ?? registration.Name;
            registration.Email = command.Email ?? registration.Email;
            registration.Phone = command.PhoneNumber ?? registration.Phone;

            await this.eventRegistrationRepository.UpdateAsync(registration);
            await this.unitOfWork.SaveChangesAsync(cancellationToken);

            // 6. Retrieve updated registration with includes
            var updatedRegistration = await this.eventRegistrationRepository.GetWithIncludesAsync(registration.Id, "Event", "User");

            this.logger.LogInformation("Event registration {EventRegistrationId} updated successfully", registration.Id);
            return Result<EventRegistration>.Success("Event registration updated successfully.", updatedRegistration, 200);
        }
    }
}
