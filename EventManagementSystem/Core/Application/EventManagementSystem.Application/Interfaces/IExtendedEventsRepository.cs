namespace EventManagementSystem.Application.Interfaces
{
    using EventManagementSystem.Domain.Models;

    /// <summary>
    /// Extended repository interface for Events with pagination and filtering capabilities.
    /// Follows Interface Segregation Principle by extending the base IRepository with specific Event operations.
    /// </summary>
    public interface IExtendedEventsRepository : IRepository<Event>
    {
        /// <summary>
        /// Gets events with pagination and filtering support.
        /// </summary>
        /// <param name="pageNumber">The page number (1-based).</param>
        /// <param name="itemsPerPage">Number of items per page.</param>
        /// <param name="searchTerm">Optional search term to filter by title, description, or location.</param>
        /// <param name="eventType">Optional event type filter.</param>
        /// <param name="startDate">Optional start date filter (events on or after this date).</param>
        /// <param name="endDate">Optional end date filter (events on or before this date).</param>
        /// <param name="isOpenForRegistration">Optional filter for registration status.</param>
        /// <param name="includes">Navigation properties to include.</param>
        /// <returns>A tuple containing the filtered events and total count.</returns>
        Task<(List<Event> Events, int TotalCount)> GetEventsWithPaginationAsync(
            int pageNumber,
            int itemsPerPage,
            string? searchTerm = null,
            string? eventType = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            bool? isOpenForRegistration = null,
            params string[] includes);
    }
}