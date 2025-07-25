namespace EventManagementSystem.Application.Interfaces
{
    using EventManagementSystem.Domain.Models;
    using System;

    public interface ITokenService
    {
        string CreateToken(AppUser user);
        DateTime GetTokenExpiration();
    }
}
