namespace EventManagementSystem.Application.Usecases.GetEvent
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Domain.Models;
    using EventManagementSystem.Application.Interfaces;

    public class GetEventQueryHandler : IRequestHandler<GetEventQuery, Result<Event>>
    {
        private readonly IRepository<Event> repository;
        private readonly IAppUserService appUserService;
        private readonly ILogger<GetEventQueryHandler> logger;

        public GetEventQueryHandler(
            IRepository<Event> repository, 
            IAppUserService appUserService,
            ILogger<GetEventQueryHandler> logger)
        {
            this.repository = repository;
            this.appUserService = appUserService;
            this.logger = logger;
        }

        public async Task<Result<Event>> Handle(GetEventQuery request, CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Fetching event with ID: {EventId}", request.EventId);

            var eventEntity = await this.repository.GetAsync(request.EventId);
            if (eventEntity == null)
            {
                this.logger.LogWarning("Event not found: {EventId}", request.EventId);
                return Result<Event>.Failure("Event not found.", null, 404, "Not Found");
            }

            eventEntity = await this.repository.GetWithIncludesAsync(request.EventId, "Image", "Registrations");

            // Manually load the AdminUser since it's in a different DbContext
            try
            {
                eventEntity!.AdminUser = await this.appUserService.GetUserAsync(eventEntity.AdminId.ToString());
                if (eventEntity.AdminUser == null)
                {
                    this.logger.LogWarning("Admin user not found for event {EventId}, AdminId: {AdminId}", 
                        request.EventId, eventEntity.AdminId);
                }
            }
            catch (System.Exception ex)
            {
                this.logger.LogWarning(ex, "Failed to load admin user for event {EventId}, AdminId: {AdminId}", 
                    request.EventId, eventEntity.AdminId);
                // Continue without admin user data
            }

            this.logger.LogInformation("Event found: {EventId}", eventEntity.Id);
            return Result<Event>.Success("Event fetched successfully.", eventEntity, 200);
        }
    }
}
