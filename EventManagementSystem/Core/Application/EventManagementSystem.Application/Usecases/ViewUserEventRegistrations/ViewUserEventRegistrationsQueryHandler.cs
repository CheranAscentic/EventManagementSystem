namespace EventManagementSystem.Application.Usecases.ViewUserEventRegistrations
{
    using MediatR;
    using Microsoft.Extensions.Logging;
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Domain.Models;
    using EventManagementSystem.Application.Interfaces;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class ViewUserEventRegistrationsQueryHandler : IRequestHandler<ViewUserEventRegistrationsQuery, Result<List<EventRegistration>>>
    {
        private readonly IAppUserService appUserService;
        private readonly IRepository<EventRegistration> eventRegistrationRepository;
        private readonly ILogger<ViewUserEventRegistrationsQueryHandler> logger;

        public ViewUserEventRegistrationsQueryHandler(
            IAppUserService appUserService,
            IRepository<EventRegistration> eventRegistrationRepository,
            ILogger<ViewUserEventRegistrationsQueryHandler> logger)
        {
            this.appUserService = appUserService;
            this.eventRegistrationRepository = eventRegistrationRepository;
            this.logger = logger;
        }

        public async Task<Result<List<EventRegistration>>> Handle(ViewUserEventRegistrationsQuery request, CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Fetching event registrations for user {AppUserId}", request.AppUserId);

            // 1. Get AppUser
            var user = await this.appUserService.GetUserAsync(request.AppUserId.ToString());
            if (user == null)
            {
                this.logger.LogWarning("User not found: {AppUserId}", request.AppUserId);
                return Result<List<EventRegistration>>.Failure("User not found.", null, 404, "Not Found");
            }

            // 2. Query EventRegistrations for user
            var registrations = await this.eventRegistrationRepository.GetAllAsync();
            var userRegistrations = registrations.Where(r => r.UserId == request.AppUserId && !r.IsCanceled).ToList();

            this.logger.LogInformation("Fetched {Count} event registrations for user {AppUserId}", userRegistrations.Count, request.AppUserId);
            return Result<List<EventRegistration>>.Success("Event registrations fetched successfully.", userRegistrations, 200);
        }
    }
}
