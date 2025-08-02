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
        private readonly ILogger<GetEventQueryHandler> logger;

        public GetEventQueryHandler(IRepository<Event> repository, ILogger<GetEventQueryHandler> logger)
        {
            this.repository = repository;
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

            eventEntity = await this.repository.GetWithIncludesAsync(request.EventId, "Image");

            this.logger.LogInformation("Event found: {EventId}", eventEntity.Id);
            return Result<Event>.Success("Event fetched successfully.", eventEntity, 200);
        }
    }
}
