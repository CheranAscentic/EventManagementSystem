using System;

namespace EventManagementSystem.Domain.Models
{
    public class EventRegistration
    {
        public Guid Id { get; set; }

        public Guid EventId { get; set; }

        public Guid UserId { get; set; }

        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public DateTime RegisteredAt { get; set; }

        public bool IsCanceled { get; set; }

        public virtual Event Event { get; set; } = null!;

        public virtual AppUser User { get; set; } = null!;
    }
}