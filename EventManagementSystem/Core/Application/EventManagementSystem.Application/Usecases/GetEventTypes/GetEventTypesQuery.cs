namespace EventManagementSystem.Application.Usecases.GetEventTypes
{
    using System.Collections.Generic;
    using EventManagementSystem.Application.DTO;
    using MediatR;

    /// <summary>
    /// Query to get all supported event types.
    /// </summary>
    public class GetEventTypesQuery : IRequest<Result<List<string>>>
    {
        // No properties needed
    }
}
