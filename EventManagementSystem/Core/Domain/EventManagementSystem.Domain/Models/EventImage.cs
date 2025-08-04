namespace EventManagementSystem.Domain.Models
{
    using System;
    using System.Text.Json.Serialization;
    using EventManagementSystem.Domain.Interfaces;

    public class EventImage : HasDto
    {
        public Guid Id { get; set; }

        public Guid EventId { get; set; }

        public string ImageUrl { get; set; } = null!;

        /// <summary>
        /// Navigation property for the event associated with this image.
        /// Must be loaded using GetWithIncludesAsync with "Event".
        /// </summary>
        [JsonIgnore]
        public virtual Event Event { get; set; } = null!;

        public object ToDto()
        {
            return new EventImageDTO
            {
                Id = this.Id,
                EventId = this.EventId,
                ImageUrl = this.ImageUrl,
            };
        }

        public class EventImageDTO
        {
            public Guid Id { get; set; }

            public Guid EventId { get; set; }

            public string ImageUrl { get; set; } = string.Empty;

        }
    }
}