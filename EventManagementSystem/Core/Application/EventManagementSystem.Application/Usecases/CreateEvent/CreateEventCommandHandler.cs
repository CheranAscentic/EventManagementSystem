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

        public CreateEventCommandHandler(
            IRepository<Event> eventRepository,
            IRepository<EventImage> eventImageRepository,
            IUnitOfWork unitOfWork,
            ILogger<CreateEventCommandHandler> logger,
            IConfiguration configuration)
        {
            this.repository = eventRepository;
            this.eventImageRepository = eventImageRepository;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.configuration = configuration;
        }

        public async Task<Result<Event>> Handle(CreateEventCommand command, CancellationToken cancellationToken = default)
        {
            this.logger.LogInformation("Creating event: {Title}", command.Title);

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
            };

            await this.repository.AddAsync(newEvent);
            await this.unitOfWork.SaveChangesAsync(cancellationToken);

            // Create default EventImage
            var defaultImageUrl = this.configuration["Default__EventImageURL"] ?? string.Empty;
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

            this.logger.LogInformation("Event created successfully: {EventId}", newEvent.Id);
            return Result<Event>.Success("Event created successfully.", eventWithImage, 201);
        }
    }
}
