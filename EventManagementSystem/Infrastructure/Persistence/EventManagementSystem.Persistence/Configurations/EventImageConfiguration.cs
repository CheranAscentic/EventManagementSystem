using EventManagementSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventManagementSystem.Persistence.Configurations
{
    public class EventImageConfiguration : IEntityTypeConfiguration<EventImage>
    {
        public void Configure(EntityTypeBuilder<EventImage> builder)
        {
            builder.HasKey(i => i.Id);
            builder.Property(i => i.ImageUrl).IsRequired().HasMaxLength(500);

            builder.HasOne(i => i.Event)
                .WithOne(e => e.Image)
                .HasForeignKey<EventImage>(i => i.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}