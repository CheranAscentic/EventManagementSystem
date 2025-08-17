namespace EventManagementSystem.Persistence.Context
{
    using EventManagementSystem.Domain.Models;
    using Microsoft.EntityFrameworkCore;

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }

        public DbSet<EventRegistration> EventRegistrations { get; set; }

        public DbSet<EventImage> EventImages { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new Configurations.EventConfiguration());
            builder.ApplyConfiguration(new Configurations.EventRegistrationConfiguration());
            builder.ApplyConfiguration(new Configurations.EventImageConfiguration());
            builder.ApplyConfiguration(new Configurations.RefreshTokenConfiguration());
        }
    }
}