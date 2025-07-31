namespace EventManagementSystem.Domain.Models
{
    public class EventImage
    {
        public Guid Id { get; set; }

        public Guid EventId { get; set; }

        public string ImageUrl { get; set; } = null!;

        public virtual Event Event { get; set; } = null!;
    }
}