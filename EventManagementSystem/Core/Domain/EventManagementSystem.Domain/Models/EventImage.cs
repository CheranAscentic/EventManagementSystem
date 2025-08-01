namespace EventManagementSystem.Domain.Models
{
    public class EventImage
    {
        public Guid Id { get; set; }

        public Guid EventId { get; set; }

        public string ImageUrl { get; set; } = null!;

        /// <summary>
        /// Navigation property for the event associated with this image.
        /// Must be loaded using GetWithIncludesAsync with "Event".
        /// </summary>
        public virtual Event Event { get; set; } = null!;
    }
}