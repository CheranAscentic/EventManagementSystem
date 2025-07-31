namespace EventManagementSystem.Application.Usecases.CreateEvent
{
    using System;
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Domain.Models;
    using MediatR;

    public class CreateEventCommand : IRequest<Result<Event>>
    {
        public CreateEventCommand(string title, string description, DateTime eventDate, string location, string type, int capacity, DateTime registrationCutoffDate)
        {
            Title = title;
            Description = description;
            EventDate = eventDate;
            Location = location;
            Type = type;
            Capacity = capacity;
            RegistrationCutoffDate = registrationCutoffDate;
        }

        public string Title { get; }

        public string Description { get; }

        public DateTime EventDate { get; }

        public string Location { get; }

        public string Type { get; }

        public int Capacity { get; }

        public DateTime RegistrationCutoffDate { get; }
    }
}
