namespace EventManagementSystem.Application.Interfaces
{
    using EventManagementSystem.Domain.Models;

    public interface ITokenService
    {
        (string Token, DateTime Expiration) CreateToken(AppUser user); // Updated to return token and expiration

        DateTime GetTokenExpiration(TimeSpan lifeTime);

        RefreshToken CreateRefreshToken(AppUser user);

        TimeSpan AuthTokenLifeSpan { get; }
        TimeSpan RefreshTokenLifeSpan { get; }
    }
}
