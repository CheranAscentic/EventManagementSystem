using EventManagementSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventManagementSystem.Persistence.Configurations
{
    public class EventRegistrationConfiguration : IEntityTypeConfiguration<EventRegistration>
    {
        public void Configure(EntityTypeBuilder<EventRegistration> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Name).IsRequired().HasMaxLength(100);
            builder.Property(r => r.Email).IsRequired().HasMaxLength(100);
            builder.Property(r => r.Phone).IsRequired().HasMaxLength(20);
            builder.Property(r => r.RegisteredAt).IsRequired();
            builder.Property(r => r.IsCanceled).IsRequired();
            builder.Property(r => r.UserId).IsRequired(); // Ensure UserId is configured

            builder.HasOne(r => r.Event)
                .WithMany(e => e.Registrations)
                .HasForeignKey(r => r.EventId);

            // Ignore cross-context navigation property
            // User navigation property references AppUser which is in IdentityDbContext
            //builder.Ignore(r => r.User);
        }
    }
}