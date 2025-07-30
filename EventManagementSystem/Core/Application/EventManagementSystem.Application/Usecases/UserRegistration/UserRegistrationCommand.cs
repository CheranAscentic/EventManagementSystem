namespace EventManagementSystem.Application.Usecases.UserRegistration
{
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Domain.Models;
    using MediatR;

    public class UserRegistrationCommand : IRequest<Result<AppUser>>
    {
        public string Email { get; } = string.Empty;

        public string Password { get; } = string.Empty;

        public string? FirstName { get; }

        public string? LastName { get; }

        public string UserName { get; } = string.Empty;

        public string? PhoneNumber { get; }

        public UserRegistrationCommand(string email, string password, string? firstName, string? lastName, string userName, string? phoneNumber)
        {
            Email = email;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
            UserName = userName;
            PhoneNumber = phoneNumber;

        }
    }
}
