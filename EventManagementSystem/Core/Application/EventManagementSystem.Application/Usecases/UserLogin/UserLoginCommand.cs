namespace EventManagementSystem.Application.Usecases.UserLogin
{
    using EventManagementSystem.Application.DTO;
    using MediatR;

    public class UserLoginCommand : IRequest<Result<LoginDTO>>
    {
        public string Email { get; } = string.Empty;

        public string Password { get; } = string.Empty;

        public UserLoginCommand(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}