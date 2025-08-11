namespace EventManagementSystem.Application.DTO
{
    using EventManagementSystem.Domain.Interfaces;

    public record LoginDTO : IsDto
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;

        public DateTimeOffset TokenExpiration { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? PhoneNumber { get; set; }

        public string userRole { get; set; } = string.Empty;
    }
}
