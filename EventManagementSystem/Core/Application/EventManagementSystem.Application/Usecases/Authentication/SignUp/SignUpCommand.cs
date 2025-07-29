namespace EventManagementSystem.Application.Usecases.Authentication.SignUp
{
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Domain.Models;
    using MediatR;

    public class SignUpCommand : IRequest<Result<AppUser>>
    {
        public string Email { get; } = string.Empty;

        public string Password { get; } = string.Empty;

        public string? FirstName { get; }

        public string? LastName { get; }

        public string UserName { get; } = string.Empty;

        public string? PhoneNumber { get; }

        public SignUpCommand(string email, string password, string? firstName, string? lastName, string userName, string? phoneNumber)
        {
            this.Email = email;
            this.Password = password;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.UserName = userName;
            this.PhoneNumber = phoneNumber;

        }
    }
}
