namespace EventManagementSystem.Application.Usecases.UpdateEvent
{
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Domain.Models;
    using MediatR;

    public class UpdateEventCommand : IRequest<Result<Event>>
    {
        public UpdateEventCommand(
            Guid id,
            string? title = null,
            string? description = null,
            DateTime? eventDate = null,
            string? location = null,
            string? type = null,
            int? capacity = null,
            DateTime? registrationCutoffDate = null)
        {
            Id = id;
            Title = title;
            Description = description;
            EventDate = eventDate;
            Location = location;
            Type = type;
            Capacity = capacity;
            RegistrationCutoffDate = registrationCutoffDate;
        }

        public Guid Id { get; }

        public string? Title { get; }

        public string? Description { get; }

        public DateTime? EventDate { get; }

        public string? Location { get; }

        public string? Type { get; }

        public int? Capacity { get; }

        public DateTime? RegistrationCutoffDate { get; }

    }
}
