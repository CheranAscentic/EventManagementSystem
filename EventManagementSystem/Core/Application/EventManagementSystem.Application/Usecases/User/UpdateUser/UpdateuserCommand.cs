using EventManagementSystem.Application.DTO;
using EventManagementSystem.Domain.Models;
using MediatR;

namespace EventManagementSystem.Application.Usecases.User.UpdateUser
{
    public class UpdateUserCommand : IRequest<StandardResponseObject<AppUser>>
    {
        public string UserId { get; } = string.Empty;
        public string UserName { get; } = string.Empty;
        public string? FirstName { get; }
        public string? LastName { get; }
        public string? PhoneNumber { get; }

        public UpdateUserCommand(string userId, string userName, string? firstName, string? lastName, string? phoneNumber)
        {
            UserId = userId;
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
        }
    }
}
