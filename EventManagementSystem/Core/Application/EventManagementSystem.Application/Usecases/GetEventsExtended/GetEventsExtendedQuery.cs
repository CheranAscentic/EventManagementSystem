namespace EventManagementSystem.Application.Usecases.GetEventsExtended
{
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Domain.Models;
    using MediatR;

    /// <summary>
    /// Query for getting events with pagination and filtering.
    /// Implements IRequest pattern from MediatR for CQRS architecture.
    /// </summary>
    public class GetEventsExtendedQuery : IRequest<Result<PaginatedResult<Event>>>
    {
        public int ItemsPerPage { get; set; }

        public int PageNumber { get; set; }

        public string? SearchTerm { get; set; }

        public string? EventType { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
