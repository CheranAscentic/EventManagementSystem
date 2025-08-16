using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using EventManagementSystem.Application.DTO;
using EventManagementSystem.Domain.Models;
using EventManagementSystem.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace EventManagementSystem.Application.Usecases.GetEventsExtended
{
    /// <summary>
    /// Handler for GetEventsExtendedQuery that provides paginated and filtered event results.
    /// Follows Single Responsibility Principle by handling only the GetEventsExtended use case.
    /// Implements Dependency Inversion Principle by depending on abstractions (IExtendedEventsRepository).
    /// </summary>
    public class GetEventsExtendedQueryHandler : IRequestHandler<GetEventsExtendedQuery, Result<PaginatedResult<Event>>>
    {
        private readonly IExtendedEventsRepository repository;
        private readonly IAppUserService appUserService;
        private readonly ILogger<GetEventsExtendedQueryHandler> logger;

        public GetEventsExtendedQueryHandler(
            IExtendedEventsRepository repository,
            IAppUserService appUserService,
            ILogger<GetEventsExtendedQueryHandler> logger)
        {
            this.repository = repository;
            this.appUserService = appUserService;
            this.logger = logger;
        }

        public async Task<Result<PaginatedResult<Event>>> Handle(
            GetEventsExtendedQuery request, 
            CancellationToken cancellationToken)
        {
            this.logger.LogInformation(
                "Fetching events with pagination - Page: {PageNumber}, ItemsPerPage: {ItemsPerPage}, SearchTerm: {SearchTerm}, EventType: {EventType}",
                request.PageNumber,
                request.ItemsPerPage,
                request.SearchTerm, 
                request.EventType);

            try
            {
                // Call the repository with all filter parameters
                var (events, totalCount) = await this.repository.GetEventsWithPaginationAsync(
                    request.PageNumber,
                    request.ItemsPerPage,
                    request.SearchTerm,
                    request.EventType,
                    request.StartDate,
                    request.EndDate,
                    null, // isOpenForRegistration - can be added to query if needed
                    "Image",
                    "Registrations"); // Include navigation properties

                // Manually load AdminUser for each event since it's in a different DbContext
                //foreach (var eventItem in events)
                //{
                //    try
                //    {
                //        eventItem.AdminUser = await this.appUserService.GetUserAsync(eventItem.AdminId.ToString());
                //        if (eventItem.AdminUser == null)
                //        {
                //            this.logger.LogWarning("Admin user not found for event {EventId}, AdminId: {AdminId}", 
                //                eventItem.Id, eventItem.AdminId);
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        this.logger.LogWarning(ex, "Failed to load admin user for event {EventId}, AdminId: {AdminId}", 
                //            eventItem.Id, eventItem.AdminId);
                //        // Continue without admin user data for this event
                //    }
                //}

                // Create paginated result
                var paginatedResult = new PaginatedResult<Event>(
                    events,
                    totalCount,
                    request.PageNumber,
                    request.ItemsPerPage);


                this.logger.LogInformation(
                    "Successfully fetched {EventCount} events out of {TotalCount} total events for page {PageNumber}",
                    events.Count,
                    totalCount,
                    request.PageNumber);

                return Result<PaginatedResult<Event>>.Success(
                    "Events fetched successfully with pagination.",
                    paginatedResult,
                    200);
            }
            catch (System.Exception)
            {
                this.logger.LogError(
                    "Error occurred while fetching events with pagination for page {PageNumber}", 
                    request.PageNumber);

                throw;
            }
        }
    }
}
