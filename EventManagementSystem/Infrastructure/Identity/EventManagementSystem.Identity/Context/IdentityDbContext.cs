namespace EventManagementSystem.Identity.Context
{
    using EventManagementSystem.Domain.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class IdentityDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
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
                    Name = "User",
                    NormalizedName = "USER",
                },
            };

            builder.Entity<IdentityRole<Guid>>().HasData(roles);
        }
    }
}
