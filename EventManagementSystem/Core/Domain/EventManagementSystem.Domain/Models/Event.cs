namespace EventManagementSystem.Domain.Models
{
    using System;
    using System.Collections.Generic;

    public class Event
    {
        public Guid Id { get; set; }

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

    }
}