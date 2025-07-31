namespace EventManagementSystem.Application.Interfaces
{
    using EventManagementSystem.Domain.Models;

    public interface ITokenService
    {
        string CreateToken(AppUser user);
        DateTime GetTokenExpiration();
    }
}
