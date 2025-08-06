namespace EventManagementSystem.Application.Usecases.ViewEventEventRegistrations
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Application.Interfaces;
    using EventManagementSystem.Domain.Models;
    using MediatR;
    using Microsoft.Extensions.Logging;

    public class ViewEventEventRegistrationsQueryHandler : IRequestHandler<ViewEventEventRegistrationsQuery, Result<List<EventRegistration>>>
    {
        private readonly IRepository<Event> eventRepository;
        private readonly ILogger<ViewEventEventRegistrationsQueryHandler> logger;
        private readonly ICurrentUserService currentUserService;

        public ViewEventEventRegistrationsQueryHandler(
            IRepository<Event> eventRepository,
            ILogger<ViewEventEventRegistrationsQueryHandler> logger,
            ICurrentUserService currentUserService)
        {
            this.eventRepository = eventRepository;
            this.logger = logger;
            this.currentUserService = currentUserService;
        }

        public async Task<Result<List<EventRegistration>>> Handle(ViewEventEventRegistrationsQuery request, CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Fetching event registrations for event {EventId}", request.EventId);

            // Get current user ID from JWT token
            var currentUserId = this.currentUserService.GetCurrentUserId();
            if (currentUserId == null)
            {
                this.logger.LogWarning("View event registrations failed: User not authenticated");
                return Result<List<EventRegistration>>.Failure("View event registrations failed", null, 401, "User not authenticated.");
            }

            // 1. Get Event
            var eventEntity = await this.eventRepository.GetAsync(request.EventId);
            if (eventEntity == null)
            {
                this.logger.LogWarning("Event not found: {EventId}", request.EventId);
                return Result<List<EventRegistration>>.Failure("Event not found.", null, 404, "Not Found");
            }

            // Check if current user is the event owner or is an admin
            var isAdmin = this.currentUserService.IsCurrentUserAdmin();
            var isEventOwner = eventEntity.AdminId == currentUserId;

            if (!isAdmin && !isEventOwner)
            {
                this.logger.LogWarning(
                    "View event registrations failed: User {UserId} is not authorized to view registrations for event {EventId}. Event owner: {EventOwnerId}", 
                    currentUserId,
                    request.EventId,
                    eventEntity.AdminId);
                return Result<List<EventRegistration>>.Failure("View event registrations failed", null, 403, "Only the event owner can view event registrations.");
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

            this.logger.LogInformation("Fetched {Count} registrations for event {EventId} by user {UserId}", noOfRegistrations, request.EventId, currentUserId);
            return Result<List<EventRegistration>>.Success("Event registrations fetched successfully.", new List<EventRegistration>(registrations), 200);
        }
    }
}
