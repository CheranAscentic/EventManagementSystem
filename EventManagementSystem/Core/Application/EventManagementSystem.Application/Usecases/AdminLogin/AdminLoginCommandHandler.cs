namespace EventManagementSystem.Application.Usecases.AdminLogin
{
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Application.Interfaces;
    using EventManagementSystem.Domain.Models;
    using MediatR;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;

    public class AdminLoginCommandHandler : IRequestHandler<AdminLoginCommand, Result<LoginDTO>>
    {
        private readonly IAppUserService appUserService;
        private readonly ITokenService tokenService;
        private readonly UserManager<AppUser> userManager;
        private readonly ILogger<AdminLoginCommandHandler> logger;

        public AdminLoginCommandHandler(
            IAppUserService appUserService,
            ITokenService tokenService,
            UserManager<AppUser> userManager,
            ILogger<AdminLoginCommandHandler> logger)
        {
            this.appUserService = appUserService;
            this.tokenService = tokenService;
            this.userManager = userManager;
            this.logger = logger;
        }

        public async Task<Result<LoginDTO>> Handle(AdminLoginCommand command, CancellationToken cancellationToken = default)
        {
            this.logger.LogInformation("Attempting admin login for Email: {Email}", command.Email);

            var user = await this.appUserService.LoginAsync(command.Email, command.Password);
            if (user == null)
            {
                this.logger.LogWarning("Admin login failed for Email: {Email}", command.Email);
                return Result<LoginDTO>.Failure("Admin login failed", null, 400, "Invalid credentials");
            }

            // Check if user is in Admin role
            var isAdmin = await this.userManager.IsInRoleAsync(user, "Admin");
            if (!isAdmin)
            {
                this.logger.LogWarning("Admin login failed: User is not an admin. Email: {Email}", command.Email);
                return Result<LoginDTO>.Failure("Admin login failed", null, 403, "User is not an admin");
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

            this.logger.LogInformation("Admin login successful for Email: {Email}", command.Email);
            return Result<LoginDTO>.Success("Admin login successful", response, 200);
        }
    }
}
