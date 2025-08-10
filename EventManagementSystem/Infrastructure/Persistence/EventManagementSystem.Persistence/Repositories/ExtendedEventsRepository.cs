namespace EventManagementSystem.Persistence.Repositories
{

    using EventManagementSystem.Application.Interfaces;
    using EventManagementSystem.Domain.Models;
    using EventManagementSystem.Persistence.Context;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Extended repository implementation for Events with pagination and filtering capabilities.
    /// Follows Single Responsibility Principle by focusing on Event-specific data access operations.
    /// Extends GenericRepository to leverage existing functionality (Open/Closed Principle).
    /// </summary>
    public class ExtendedEventsRepository : GenericRepository<Event>, IExtendedEventsRepository
    {
        private readonly ApplicationDbContext context;

        public ExtendedEventsRepository(ApplicationDbContext context)
            : base(context)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets events with pagination and filtering support.
        /// Implements the Repository pattern with specific filtering logic for Events.
        /// </summary>
        public async Task<(List<Event> Events, int TotalCount)> GetEventsWithPaginationAsync(
            int pageNumber,
            int itemsPerPage,
            string? searchTerm = null,
            string? eventType = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            bool? isOpenForRegistration = null,
            params string[] includes)
        {
            // Start with the base query
            IQueryable<Event> query = this.context.Set<Event>();

            // Apply includes for navigation properties
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            // Apply filters using method chaining for better readability and maintainability
            query = ApplySearchFilter(query, searchTerm);
            query = ApplyEventTypeFilter(query, eventType);
            query = ApplyDateRangeFilter(query, startDate, endDate);
            query = ApplyRegistrationStatusFilter(query, isOpenForRegistration);

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var events = await query
                .OrderBy(e => e.EventDate) // Default ordering by event date
                .Skip((pageNumber - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToListAsync();

            return (events, totalCount);
        }

        /// <summary>
        /// Applies search filter across multiple fields (Title, Description, Location).
        /// Follows Single Responsibility Principle by handling only search logic.
        /// </summary>
        private static IQueryable<Event> ApplySearchFilter(IQueryable<Event> query, string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return query;
            }

            var lowerSearchTerm = searchTerm.ToLower();
            return query.Where(e =>
                e.Title.ToLower().Contains(lowerSearchTerm) ||
                e.Description.ToLower().Contains(lowerSearchTerm) ||
                e.Location.ToLower().Contains(lowerSearchTerm));
        }

        /// <summary>
        /// Applies event type filter.
        /// Follows Single Responsibility Principle by handling only event type filtering.
        /// </summary>
        private static IQueryable<Event> ApplyEventTypeFilter(IQueryable<Event> query, string? eventType)
        {
            if (string.IsNullOrWhiteSpace(eventType))
            {
                return query;
            }

            return query.Where(e => e.Type == eventType);
        }

        /// <summary>
        /// Applies date range filter for event dates.
        /// Follows Single Responsibility Principle by handling only date range filtering.
        /// </summary>
        private static IQueryable<Event> ApplyDateRangeFilter(IQueryable<Event> query, DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue)
            {
                query = query.Where(e => e.EventDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(e => e.EventDate <= endDate.Value);
            }

            return query;
        }

        /// <summary>
        /// Applies registration status filter.
        /// Follows Single Responsibility Principle by handling only registration status filtering.
        /// </summary>
        private static IQueryable<Event> ApplyRegistrationStatusFilter(IQueryable<Event> query, bool? isOpenForRegistration)
        {
            if (!isOpenForRegistration.HasValue) 
            {
                return query;
            }

            return query.Where(e => e.IsOpenForRegistration == isOpenForRegistration.Value);
        }
    }
}
