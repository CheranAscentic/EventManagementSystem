namespace EventManagementSystem.Domain.Models
{
    using System;
    using System.Collections.Generic;
    using EventManagementSystem.Domain.Interfaces;

    public class Event : HasDto
    {
        public Guid Id { get; set; }

        public Guid AdminId { get; set; }

        public string AdminName { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public DateTime EventDate { get; set; }

        public string Location { get; set; } = null!;

        public string Type { get; set; } = null!;

        public int Capacity { get; set; }

        public bool IsOpenForRegistration { get; set; } = true;

        public DateTime RegistrationCutoffDate { get; set; }

        /// <summary>
        /// Navigation property for the registrations associated with this event.
        /// Must be loaded using GetWithIncludesAsync with "Registrations" (and "Registrations.User" for user details).
        /// </summary>
        public virtual ICollection<EventRegistration> Registrations { get; set; } = new List<EventRegistration>();

        /// <summary>
        /// Navigation property for the image associated with this event.
        /// Must be loaded using GetWithIncludesAsync with "Image".
        /// </summary>
        public virtual EventImage? Image { get; set; }

        public int NoOfRegistrations { get => this.Registrations?.Count ?? 0; }

        public object ToDto()
        {
            return new EventDTO
            {
                Id = this.Id,
                AdminId = this.AdminId,
                Title = this.Title,
                Description = this.Description,
                EventDate = this.EventDate,
                Location = this.Location,
                Type = this.Type,
                Capacity = this.Capacity,
                IsOpenForRegistration = this.IsOpenForRegistration,
                RegistrationCutoffDate = this.RegistrationCutoffDate,
                NoOfRegistrations = this.NoOfRegistrations,
                ImageUrl = this.Image?.ImageUrl ?? string.Empty,
                RegisteredIds = this.Registrations?.Select(r => r.UserId).ToList(),
                Owner = this.AdminName,
            };
        }

        public class EventDTO : IsDto
        {
            public Guid Id { get; set; }

            public Guid AdminId { get; set; }

            public string Title { get; set; } = string.Empty;

            public string Description { get; set; } = string.Empty;

            public DateTime EventDate { get; set; }

            public string Location { get; set; } = string.Empty;

            public string Type { get; set; } = string.Empty;

            public int Capacity { get; set; }

            public bool IsOpenForRegistration { get; set; }

            public DateTime RegistrationCutoffDate { get; set; }

            public int NoOfRegistrations { get; set; }

            public string? ImageUrl { get; set; }

            public List<Guid>? RegisteredIds { get; set; }

            public string? Owner { get; set; }
        }
    }
}
