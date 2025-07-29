namespace EventManagementSystem.Application.Usecases.Authentication.Login
{
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Application.Interfaces;
    using EventManagementSystem.Domain.Models;
    using MediatR;
    using Microsoft.Extensions.Logging;

    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginDTO>>
    {
        private readonly IAppUserService appUserService;
        private readonly ILogger<LoginCommandHandler> logger;
        private readonly ITokenService tokenService;

        public LoginCommandHandler(
            IAppUserService appUserService,
            ITokenService tokenService,
            ILogger<LoginCommandHandler> logger)
        {
            this.appUserService = appUserService;
            this.logger = logger;
            this.tokenService = tokenService;
        }

        public async Task<Result<LoginDTO>> Handle(LoginCommand command, CancellationToken cancellationToken = default)
        {
            this.logger.LogInformation("Attempting login for Email: {Email}", command.Email);

            var user = await this.appUserService.LoginAsync(command.Email, command.Password);
            if (user == null)
            {
                this.logger.LogWarning("Login failed for Email: {Email}", command.Email);
                return Result<LoginDTO>.Failure("Login failed", null, 400, "Invalid credentials");
            }

            // Generate token and expiration
            var token = this.tokenService.CreateToken(user);
            var tokenExpiration = this.tokenService.GetTokenExpiration();

            // Create DTO
            var response = new LoginDTO
            {
                Id = user.Id,
                Email = user.Email!,
                UserName = user.UserName!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Token = token,
                TokenExpiration = tokenExpiration,
            };

            this.logger.LogInformation("Login successful for Email: {Email}", command.Email);
            return Result<LoginDTO>.Success("Login successful", response, 200);
        }
    }
}