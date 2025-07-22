using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EventManagementSystem.Identity.Models;

namespace EventManagementSystem.Identity.Context
{
    public class IdentityDbContext : IdentityDbContext<AppUser, IdentityRole, string>
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure AppUser entity
            builder.Entity<AppUser>(entity =>
            {
                entity.Property(e => e.FirstName).HasMaxLength(100);
                entity.Property(e => e.LastName).HasMaxLength(100);
            });

            // Seed default roles (example)
            var roles = new[]
            {
                new IdentityRole<Guid>
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                },
                new IdentityRole<Guid>
                {
                    Name = "Attendee",
                    NormalizedName = "ATTENDEE",
                },
            };

            builder.Entity<IdentityRole<Guid>>().HasData(roles);
        }
    }
}