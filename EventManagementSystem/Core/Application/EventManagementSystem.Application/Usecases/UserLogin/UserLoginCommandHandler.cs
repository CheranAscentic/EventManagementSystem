namespace EventManagementSystem.Application.Usecases.UserLogin
{
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Application.Interfaces;
    using EventManagementSystem.Domain.Models;
    using MediatR;
    using Microsoft.Extensions.Logging;

    public class UserLoginCommandHandler : IRequestHandler<UserLoginCommand, Result<LoginDTO>>
    {
        private readonly IAppUserService appUserService;
        private readonly ILogger<UserLoginCommandHandler> logger;
        private readonly ITokenService tokenService;

        public UserLoginCommandHandler(
            IAppUserService appUserService,
            ITokenService tokenService,
            ILogger<UserLoginCommandHandler> logger)
        {
            this.appUserService = appUserService;
            this.logger = logger;
            this.tokenService = tokenService;
        }

        public async Task<Result<LoginDTO>> Handle(UserLoginCommand command, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Attempting login for Email: {Email}", command.Email);

            var user = await appUserService.LoginAsync(command.Email, command.Password);
            if (user == null)
            {
                logger.LogWarning("Login failed for Email: {Email}", command.Email);
                return Result<LoginDTO>.Failure("Login failed", null, 400, "Invalid credentials");
            }

            // Generate token and expiration
            var token = tokenService.CreateToken(user);
            var tokenExpiration = tokenService.GetTokenExpiration();

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

            logger.LogInformation("Login successful for Email: {Email}", command.Email);
            return Result<LoginDTO>.Success("Login successful", response, 200);
        }
    }
}