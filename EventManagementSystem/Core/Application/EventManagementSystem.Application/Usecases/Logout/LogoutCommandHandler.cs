namespace EventManagementSystem.Application.Usecases.Logout
{
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Application.Interfaces;
    using EventManagementSystem.Domain.Models;
    using MediatR;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;

    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<object>>
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IRefreshTokenRepository refreshTokenRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<LogoutCommandHandler> logger;

        public LogoutCommandHandler(
            UserManager<AppUser> userManager,
            IRefreshTokenRepository refreshTokenRepository,
            IUnitOfWork unitOfWork,
            ILogger<LogoutCommandHandler> logger)
        {
            this.userManager = userManager;
            this.refreshTokenRepository = refreshTokenRepository;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public async Task<Result<object>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Attempting logout for UserId: {UserId}", request.UserId);
            this.logger.LogDebug("Logout command received for UserId: {UserId}", request.UserId);

            // Get user using UserManager to validate the user exists
            var user = await this.userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                this.logger.LogWarning("Logout failed for UserId: {UserId} - User not found", request.UserId);
                return Result<object>.Failure("Logout failed", null, 404, "User not found");
            }

            this.logger.LogDebug("User found for logout. Email: {Email}", user.Email);

            // Delete all refresh tokens for the user
            var deletedTokensCount = await this.refreshTokenRepository.RemoveAllUserTokensAsync(request.UserId);
            this.logger.LogDebug("Deleted {TokenCount} refresh tokens for UserId: {UserId}", deletedTokensCount, request.UserId);

            // Save changes using UnitOfWork
            await this.unitOfWork.SaveChangesAsync(cancellationToken);

            this.logger.LogInformation("Logout completed successfully for UserId: {UserId}, Email: {Email}. Deleted {TokenCount} refresh tokens.", request.UserId, user.Email, deletedTokensCount);

            return Result<object>.Success("Logout successful", null, 200);
        }
    }
}
