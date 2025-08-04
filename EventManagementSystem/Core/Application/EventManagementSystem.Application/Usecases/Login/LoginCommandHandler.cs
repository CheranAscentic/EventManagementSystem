using EventManagementSystem.Application.DTO;
using EventManagementSystem.Application.Interfaces;
using EventManagementSystem.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace EventManagementSystem.Application.Usecases.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginDTO>>
    {
        private readonly IAppUserService appUserService;
        private readonly ITokenService tokenService;
        private readonly UserManager<AppUser> userManager;
        private readonly ILogger<LoginCommandHandler> logger;

        public LoginCommandHandler(
            IAppUserService appUserService,
            ITokenService tokenService,
            UserManager<AppUser> userManager,
            ILogger<LoginCommandHandler> logger)
        {
            this.appUserService = appUserService;
            this.tokenService = tokenService;
            this.userManager = userManager;
            this.logger = logger;
        }

        public async Task<Result<LoginDTO>> Handle(LoginCommand command, CancellationToken cancellationToken = default)
        {
            this.logger.LogInformation("Attempting login for Email: {Email}", command.Email);
            this.logger.LogDebug("Login command received for email: {Email}", command.Email);

            // Attempt to authenticate user
            var user = await this.appUserService.LoginAsync(command.Email, command.Password);
            if (user == null)
            {
                this.logger.LogWarning("Login failed for Email: {Email} - Invalid credentials", command.Email);
                return Result<LoginDTO>.Failure("Login failed", null, 401, "Invalid email or password");
            }

            this.logger.LogDebug("User authenticated successfully for Email: {Email}", command.Email);

            // Get user role
            var userRole = await this.appUserService.GetUserRoleAsync(user);

            // Check user roles for logging purposes
            var isAdmin = await this.userManager.IsInRoleAsync(user, "Admin");
            var isUser = await this.userManager.IsInRoleAsync(user, "User");

            if (isAdmin)
            {
                this.logger.LogInformation("Admin user logged in successfully. Email: {Email}", command.Email);
            }
            else if (isUser)
            {
                this.logger.LogInformation("Regular user logged in successfully. Email: {Email}", command.Email);
            }
            else
            {
                this.logger.LogWarning("User has no recognized role. Email: {Email}", command.Email);
            }

            // Generate token and expiration
            var token = this.tokenService.CreateToken(user);
            var tokenExpiration = this.tokenService.GetTokenExpiration();

            this.logger.LogDebug("JWT token generated successfully for Email: {Email}", command.Email);

            // Create response DTO
            var response = new LoginDTO
            {
                Id = user.Id,
                Email = user.Email!,
                UserName = user.UserName!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                userRole = userRole,
                Token = token,
                TokenExpiration = tokenExpiration,
            };

            this.logger.LogInformation("Login completed successfully for Email: {Email}", command.Email);
            return Result<LoginDTO>.Success("Login successful", response, 200);
        }
    }
}