namespace EventManagementSystem.Persistence.Configurations
{
    using EventManagementSystem.Domain.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Title).IsRequired().HasMaxLength(200);
            builder.Property(e => e.Description).IsRequired();
            builder.Property(e => e.Location).IsRequired().HasMaxLength(200);
            builder.Property(e => e.Type).IsRequired().HasMaxLength(100);
            builder.Property(e => e.Capacity).IsRequired();
            builder.Property(e => e.IsOpenForRegistration).IsRequired();
            builder.Property(e => e.RegistrationCutoffDate).IsRequired();
            builder.Property(e => e.AdminId).IsRequired();
            builder.Property(e => e.AdminName).IsRequired().HasMaxLength(200);

            // Ignore computed properties and cross-context navigation properties
            builder.Ignore(e => e.NoOfRegistrations);
            //builder.Ignore(e => e.AdminUser); // AdminUser is in IdentityDbContext, cannot create FK relationship

            builder.HasOne(e => e.Image)
                .WithOne(i => i.Event)
                .HasForeignKey<EventImage>(i => i.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Registrations)
                .WithOne(r => r.Event)
                .HasForeignKey(r => r.EventId);
        }
    }
}