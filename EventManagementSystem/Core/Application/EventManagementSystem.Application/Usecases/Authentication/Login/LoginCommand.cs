namespace EventManagementSystem.Application.Usecases.Authentication.Login
{
    using EventManagementSystem.Application.DTO;
    using MediatR;

    public class LoginCommand : IRequest<Result<LoginDTO>>
    {
        public string Email { get; } = string.Empty;

        public string Password { get; } = string.Empty;

        public LoginCommand(string email, string password)
        {
            this.Email = email;
            this.Password = password;
        }
    }
}