using EventManagementSystem.Application.DTO;
using EventManagementSystem.Domain.Models;
using MediatR;

namespace EventManagementSystem.Application.Usecases.User.GetUser
{
    public class GetUserQuery : IRequest<StandardResponseObject<AppUser>>
    {
        public string UserId { get; } = string.Empty;

        public GetUserQuery(string userId)
        {
            UserId = userId;
        }
    }
}
