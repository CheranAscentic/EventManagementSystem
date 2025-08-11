namespace EventManagementSystem.Application.Usecases.GetEvents
{
    using MediatR;
    using Microsoft.Extensions.Logging;
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Domain.Models;
    using EventManagementSystem.Application.Interfaces;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class GetEventsQueryHandler : IRequestHandler<GetEventsQuery, Result<List<Event>>>
    {
        private readonly IRepository<Event> repository;
        private readonly ILogger<GetEventsQueryHandler> logger;

        public GetEventsQueryHandler(IRepository<Event> repository, ILogger<GetEventsQueryHandler> logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        public async Task<Result<List<Event>>> Handle(GetEventsQuery request, CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Fetching all events");

            var events = await this.repository.GetAllWithIncludesAsync("Image");

            if (events == null || events.Count == 0)
            {
                this.logger.LogWarning("No events found");
                return Result<List<Event>>.Failure("No events found.", null, 404, "Not Found");
            }

            this.logger.LogInformation("Fetched {Count} events", events.Count);
            return Result<List<Event>>.Success("Events fetched successfully.", events, 200);
        }
    }
}
