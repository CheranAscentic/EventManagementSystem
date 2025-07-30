namespace EventManagementSystem.Application.Usecases.AdminLogin
{
    using EventManagementSystem.Application.DTO;
    using MediatR;

    public class AdminLoginCommand : IRequest<Result<LoginDTO>>
    {
        public AdminLoginCommand(string email, string password)
        {
            this.Email = email;
            this.Password = password;
        }

        public string Email { get; } = string.Empty;

        public string Password { get; } = string.Empty;
    }
}
