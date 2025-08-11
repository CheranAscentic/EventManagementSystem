namespace EventManagementSystem.Domain.Models
{
    using System;
    using System.Text.Json.Serialization;
    using EventManagementSystem.Domain.Interfaces;

    public class EventRegistration : HasDto
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

        public object ToDto()
        {
            return new EventRegistrationDTO
            {
                Id = this.Id,
                EventId = this.EventId,
                UserId = this.UserId,
                Name = this.Name,
                Email = this.Email,
                Phone = this.Phone,
                RegisteredAt = this.RegisteredAt,
                IsCanceled = this.IsCanceled,
            };
        }

        public class EventRegistrationDTO
        {
            public Guid Id { get; set; }

            public Guid EventId { get; set; }

            public Guid UserId { get; set; }

            public string Name { get; set; } = string.Empty;

            public string Email { get; set; } = string.Empty;

            public string Phone { get; set; } = string.Empty;

            public DateTime RegisteredAt { get; set; }

            public bool IsCanceled { get; set; }

        }
    }
}