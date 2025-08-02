using System.Text.Json.Serialization;

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

        /// <summary>
        /// Navigation property for the event associated with this registration.
        /// Must be loaded using GetWithIncludesAsync with "Event".
        /// </summary>
        [JsonIgnore]
        public virtual Event Event { get; set; } = null!;

        /// <summary>
        /// Navigation property for the user associated with this registration.
        /// Must be loaded using GetWithIncludesAsync with "User".
        /// </summary>
        public virtual AppUser User { get; set; } = null!;
    }
}