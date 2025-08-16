namespace EventManagementSystem.Application.Usecases.RefreshToken
{
    using EventManagementSystem.Application.DTO;
    using EventManagementSystem.Application.Interfaces;
    using EventManagementSystem.Domain.Models;
    using MediatR;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;
    using System.Threading;
    using System.Threading.Tasks;
    using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<CredentialsDTO>>
    {
        private readonly IRefreshTokenRepository refreshTokenRepository;
        private readonly IAppUserService appUserService;
        private readonly ITokenService tokenService;
        private readonly UserManager<AppUser> userManager;
        private readonly ILogger<RefreshTokenCommandHandler> logger;
        private readonly IUnitOfWork unitOfWork;

        public RefreshTokenCommandHandler(
            IRefreshTokenRepository refreshTokenRepository,
            IAppUserService appUserService,
            ITokenService tokenService,
            UserManager<AppUser> userManager,
            ILogger<RefreshTokenCommandHandler> logger,
            IUnitOfWork unitOfWork)
        {
            this.refreshTokenRepository = refreshTokenRepository;
            this.appUserService = appUserService;
            this.tokenService = tokenService;
            this.userManager = userManager;
            this.logger = logger;
            this.unitOfWork = unitOfWork;
        }

        public async Task<Result<CredentialsDTO>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var refreshTokenObj = await this.refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

            if (refreshTokenObj == null)
            {
                this.logger.LogWarning("Refresh Token is invalid");
                return Result<CredentialsDTO>.Failure("Refresh token Failed", null, 401, "Invalid Refresh Token");
            }

            if (!refreshTokenObj.IsActive)
            {
                this.logger.LogWarning("Refresh Token is Expired");
                return Result<CredentialsDTO>.Failure("Refresh token Failed", null, 401, "Refresh Token Is Expired");
            }

            if (refreshTokenObj.IsRevoked)
            {
                this.logger.LogWarning("Refresh Token was Revoked");
                return Result<CredentialsDTO>.Failure("Refresh token Failed", null, 401, "Refresh Token Is Revoked");
            }

            var user = await this.appUserService.GetUserAsync(refreshTokenObj.AppUserId.ToString());

            if (user == null)
            {
                this.logger.LogWarning("User not found for Refresh Token");
                return Result<CredentialsDTO>.Failure("Refresh token Failed", null, 404, "User not found");
            }

            var (authToken, authTokenExp) = this.tokenService.CreateToken(user);

            this.logger.LogDebug("New Auth Token token generated successfully for Email: {Email}", user.Email);

            var refreshToken = this.tokenService.CreateRefreshToken(user);

            this.logger.LogDebug("New Refresh token generated successfully for Email: {Email}", user.Email);

            var newRefreshTokenObj = new RefreshToken
            {
                Token = refreshToken.Token,
                AppUserId = user.Id,
                Expires = refreshToken.Expires,
            };

            await this.refreshTokenRepository.AddAsync(newRefreshTokenObj);

            var response = new CredentialsDTO
            {
                AuthToken = authToken,
                RefreshToken = refreshToken.Token,
                AuthTokenExp = authTokenExp.ToUniversalTime().ToString("o"),
                RefreshTokenExp = refreshToken.Expires.ToUniversalTime().ToString("o"),
            };

            await this.unitOfWork.SaveChangesAsync(cancellationToken);

            this.logger.LogInformation("Token Refresh completed successfully for Email: {Email}", user.Email);
            return Result<CredentialsDTO>.Success("Token refresh successful", response, 200);
        }
    }
}
