namespace EventManagementSystem.Application.Usecases.EventCreation
{
    using System;
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Domain.Models;
    using MediatR;

    public class CreateEventCommand : IRequest<Result<Event>>
    {
        public CreateEventCommand(string title, string description, DateTime eventDate, string location, string type, int capacity, DateTime registrationCutoffDate)
        {
            this.Title = title;
            this.Description = description;
            this.EventDate = eventDate;
            this.Location = location;
            this.Type = type;
            this.Capacity = capacity;
            this.RegistrationCutoffDate = registrationCutoffDate;
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
