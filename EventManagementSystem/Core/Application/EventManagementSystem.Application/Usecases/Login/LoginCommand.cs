namespace EventManagementSystem.Application.Usecases.Login
{
    using EventManagementSystem.Application.DTO;
    using MediatR;

    public class LoginCommand : IRequest<Result<CredentialsDTO>>
    {
        public LoginCommand(string email, string password)
        {
            this.Email = email;
            this.Password = password;
        }

        public string Email { get; }

        public string Password { get; }
    }
}
