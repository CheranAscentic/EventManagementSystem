namespace EventManagementSystem.Application.Usecases.User.DeleteUser
{
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Domain.Models;
    using MediatR;

    public class DeleteUserCommand : IRequest<Result<AppUser>>
    {
        public string UserId { get; }

        public DeleteUserCommand(string userId)
        {
            UserId = userId;
        }
    }
}
