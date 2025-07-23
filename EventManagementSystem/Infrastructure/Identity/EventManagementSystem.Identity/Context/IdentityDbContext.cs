namespace EventManagementSystem.Identity.Context
{
    using EventManagementSystem.Domain.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Represents the database context for identity management, including users and roles.
    /// </summary>
    public class IdentityDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityDbContext"/> class.
        /// </summary>
        /// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Configures the schema needed for the identity framework.
        /// </summary>
        /// <param name="builder">The builder being used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure AppUser entity
            builder.Entity<AppUser>(entity =>
            {
                entity.Property(e => e.FirstName).HasMaxLength(100);
                entity.Property(e => e.LastName).HasMaxLength(100);
            });

            // Seed default roles
            var roles = new[]
            {
                new IdentityRole<Guid>
                {
                    Id = Guid.Parse("411d52bb-b369-4a31-81c7-d149317f8105"),
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                },
                new IdentityRole<Guid>
                {
                    Id = Guid.Parse("c1122f1a-3748-466b-af45-ac3992d2e121"),
                    Name = "Attendee",
                    NormalizedName = "ATTENDEE",
                },
            };

            builder.Entity<IdentityRole<Guid>>().HasData(roles);
        }
    }
}