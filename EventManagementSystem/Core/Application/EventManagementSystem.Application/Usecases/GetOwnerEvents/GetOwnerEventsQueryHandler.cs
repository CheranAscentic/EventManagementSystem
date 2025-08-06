namespace EventManagementSystem.Application.Usecases.GetOwnerEvents
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Application.Interfaces;
    using EventManagementSystem.Domain.Models;
    using MediatR;
    using Microsoft.Extensions.Logging;

    public class GetOwnerEventsQueryHandler : IRequestHandler<GetOwnerEventsQuery, Result<List<Event>>>
    {
        private readonly IRepository<Event> repository;
        private readonly IAppUserService appUserService;
        private readonly ILogger<GetOwnerEventsQueryHandler> logger;

        public GetOwnerEventsQueryHandler(
            IRepository<Event> repository,
            IAppUserService appUserService,
            ILogger<GetOwnerEventsQueryHandler> logger)
        {
            this.repository = repository;
            this.appUserService = appUserService;
            this.logger = logger;
        }

        public async Task<Result<List<Event>>> Handle(GetOwnerEventsQuery request, CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Fetching events for owner: {OwnerId}", request.OwnerId);

            // Get the user to validate existence and role
            var user = await this.appUserService.GetUserAsync(request.OwnerId.ToString());
            if (user == null)
            {
                this.logger.LogWarning("User not found: {OwnerId}", request.OwnerId);
                return Result<List<Event>>.Failure("User not found.", null, 404, "User with the specified ID does not exist.");
            }

            // Check if user is admin
            var userRole = await this.appUserService.GetUserRoleAsync(user);
            if (userRole != "Admin")
            {
                this.logger.LogWarning("User {OwnerId} is not an admin. Role: {Role}", request.OwnerId, userRole);
                return Result<List<Event>>.Failure("Access denied.", null, 403, "Only admin users can own events.");
            }

            this.logger.LogInformation("User {OwnerId} validated as admin. Fetching events.", request.OwnerId);

            // Get all events and filter by AdminId
            var allEvents = await this.repository.GetAllWithIncludesAsync("Image");
            var ownerEvents = allEvents.Where(e => e.AdminId == request.OwnerId).ToList();

            if (ownerEvents.Count == 0)
            {
                this.logger.LogInformation("No events found for owner: {OwnerId}", request.OwnerId);
                return Result<List<Event>>.Success("No events found for this owner.", new List<Event>(), 200);
            }

            this.logger.LogInformation("Successfully fetched {Count} events for owner: {OwnerId}", ownerEvents.Count, request.OwnerId);
            return Result<List<Event>>.Success("Owner events fetched successfully.", ownerEvents, 200);
        }
    }
}