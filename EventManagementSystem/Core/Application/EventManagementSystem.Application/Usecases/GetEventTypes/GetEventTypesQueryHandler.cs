namespace EventManagementSystem.Application.Usecases.GetEventTypes
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Domain.Enums;
    using MediatR;
    using Microsoft.Extensions.Logging;

    public class GetEventTypesQueryHandler : IRequestHandler<GetEventTypesQuery, Result<List<string>>>
    {
        private readonly ILogger<GetEventTypesQueryHandler> logger;

        public GetEventTypesQueryHandler(ILogger<GetEventTypesQueryHandler> logger)
        {
            this.logger = logger;
        }

        public Task<Result<List<string>>> Handle(GetEventTypesQuery request, CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Fetching all event types");

            var types = EventType.Types;
            if (types == null || types.Count == 0)
            {
                this.logger.LogWarning("No event types found");
                return Task.FromResult(Result<List<string>>.Failure("No event types found.", null, 404, "Not Found"));
            }

            this.logger.LogInformation("Fetched {Count} event types", types.Count);
            return Task.FromResult(Result<List<string>>.Success("Event types fetched successfully.", types, 200));
        }
    }
}
