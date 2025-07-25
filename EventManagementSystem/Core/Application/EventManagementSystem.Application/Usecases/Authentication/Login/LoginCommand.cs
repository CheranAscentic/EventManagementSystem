using EventManagementSystem.Application.DTO;
using EventManagementSystem.Domain.Models;
using MediatR;

namespace EventManagementSystem.Application.Usecases.Authentication.Login
{
    /// <summary>
    /// Represents a query to log in a user.
    /// </summary>
    public class LoginCommand : IRequest<StandardResponseObject<LoginDTO>>
    {
        public string Email { get; }
        public string Password { get; }

        public LoginCommand(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}