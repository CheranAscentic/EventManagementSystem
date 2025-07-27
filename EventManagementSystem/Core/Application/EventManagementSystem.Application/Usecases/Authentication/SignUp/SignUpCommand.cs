namespace EventManagementSystem.Application.Usecases.Authentication.SignUp
{
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Domain.Models;
    using MediatR;

    /// <summary>
    /// Represents a command to sign up a user.
    /// </summary>
    public class SignUpCommand : IRequest<StandardResponseObject<AppUser>>
    {
        public string Email { get; }

        public string Password { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public string UserName { get; }

        public string PhoneNumber { get; }

    }
}
