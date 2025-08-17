namespace EventManagementSystem.Application.DTO
{
    using EventManagementSystem.Domain.Interfaces;

    public record CredentialsDTO : IsDto
    {
        public string AuthToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string AuthTokenExp { get; set; } = string.Empty; // ISO string
        public string RefreshTokenExp { get; set; } = string.Empty; // ISO string
    }
}
