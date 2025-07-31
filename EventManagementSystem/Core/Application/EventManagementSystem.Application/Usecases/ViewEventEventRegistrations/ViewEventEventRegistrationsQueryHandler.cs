namespace EventManagementSystem.Application.Usecases.ViewEventEventRegistrations
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Domain.Models;
    using EventManagementSystem.Application.Interfaces;

    public class ViewEventEventRegistrationsQueryHandler : IRequestHandler<ViewEventEventRegistrationsQuery, Result<List<EventRegistration>>>
    {
        private readonly IRepository<Event> eventRepository;
        private readonly ILogger<ViewEventEventRegistrationsQueryHandler> logger;

        public ViewEventEventRegistrationsQueryHandler(
            IRepository<Event> eventRepository,
            ILogger<ViewEventEventRegistrationsQueryHandler> logger)
        {
            this.eventRepository = eventRepository;
            this.logger = logger;
        }

        public async Task<Result<List<EventRegistration>>> Handle(ViewEventEventRegistrationsQuery request, CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Fetching event registrations for event {EventId}", request.EventId);

            // 1. Get Event
            var eventEntity = await this.eventRepository.GetAsync(request.EventId);
            if (eventEntity == null)
            {
                this.logger.LogWarning("Event not found: {EventId}", request.EventId);
                return Result<List<EventRegistration>>.Failure("Event not found.", null, 404, "Not Found");
            }

            // 2. Get Event with Registrations
            eventEntity = await this.eventRepository.GetWithIncludesAsync(request.EventId, "Registrations");
            if (eventEntity == null)
            {
                this.logger.LogWarning("Event with registrations not found: {EventId}", request.EventId);
                return Result<List<EventRegistration>>.Failure("Event not found.", null, 404, "Not Found");
            }

            var registrations = eventEntity.Registrations;
            var noOfRegistrations = eventEntity.NoOfRegistrations;

            this.logger.LogInformation("Fetched {Count} registrations for event {EventId}", noOfRegistrations, request.EventId);
            return Result<List<EventRegistration>>.Success("Event registrations fetched successfully.", new List<EventRegistration>(registrations), 200);
        }
    }
}
