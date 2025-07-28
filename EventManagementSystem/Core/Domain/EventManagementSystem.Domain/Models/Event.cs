namespace EventManagementSystem.Domain.Models
{
    using System;
    using System.Collections.Generic;

    public class Event
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public DateTime Date { get; set; }

        public string Location { get; set; } = null!;

        public string Type { get; set; } = null!;

        public int Capacity { get; set; }

        public bool IsOpenForRegistration { get; set; }

        public DateTime RegistrationCutoff { get; set; }

        public virtual ICollection<EventRegistration> Registrations { get; set; } = new List<EventRegistration>();

        public virtual EventImage? Image { get; set; }
    }
}