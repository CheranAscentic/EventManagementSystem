namespace EventManagementSystem.Persistence.Configurations
{
    using EventManagementSystem.Domain.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            // Configure table name
            builder.ToTable("RefreshTokens");

            // Token as primary key
            builder.HasKey(rt => rt.Token);

            // Token property configuration
            builder.Property(rt => rt.Token)
                .IsRequired()
                .HasMaxLength(500)
                .ValueGeneratedNever(); // Important: Don't auto-generate the key

            // AppUserId configuration
            builder.Property(rt => rt.AppUserId)
                .IsRequired();

            // Created property configuration
            builder.Property(rt => rt.Created)
                .IsRequired();

            // Expires property configuration
            builder.Property(rt => rt.Expires)
                .IsRequired();

            // Revoked property (optional)
            builder.Property(rt => rt.Revoked)
                .IsRequired(false);

            // Create index on AppUserId for performance (getting all tokens for a user)
            builder.HasIndex(rt => rt.AppUserId)
                .HasDatabaseName("IX_RefreshTokens_AppUserId");

            // Create index for active tokens (performance optimization)
            builder.HasIndex(rt => new { rt.AppUserId, rt.Revoked, rt.Expires })
                .HasDatabaseName("IX_RefreshTokens_Active");
        }
    }
}
