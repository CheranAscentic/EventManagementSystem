namespace EventManagementSystem.Domain.Models
{
    public class RefreshToken
    {
        // Remove the Guid Id - Token becomes the primary key
        public string Token { get; set; } = string.Empty;

        public Guid AppUserId { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;

        public DateTime Expires { get; set; } = DateTime.UtcNow.AddDays(7); // Consider longer expiry

        public DateTime? Revoked { get; set; }

        public bool IsExpired => DateTime.UtcNow >= Expires;

        public bool IsRevoked => this.Revoked != null;

        public bool IsActive => !this.IsRevoked && !this.IsExpired;
    }
}
