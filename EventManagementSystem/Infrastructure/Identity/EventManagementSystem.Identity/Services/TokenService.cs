namespace EventManagementSystem.Identity.Services
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using EventManagementSystem.Application.Interfaces;
    using EventManagementSystem.Domain.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;

    public class TokenService : ITokenService
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<AppUser> userManager;
        private readonly TimeSpan authTokenLifeSpan;
        private readonly TimeSpan refreshTokenLifeSpan;

        public TokenService(IConfiguration config, UserManager<AppUser> userManager)
        {
            this.configuration = config;
            this.userManager = userManager;

            // Get auth token lifespan from env, fallback to 10 minutes
            var authTokenMinutesStr = Environment.GetEnvironmentVariable("AUTH_TOKEN_LIFESPAN_MINUTES");
            if (!int.TryParse(authTokenMinutesStr, out var authTokenMinutes) || authTokenMinutes <= 0)
            {
                authTokenMinutes = 10;
            }

            this.authTokenLifeSpan = TimeSpan.FromMinutes(authTokenMinutes);

            // Get refresh token lifespan from env, fallback to 12 hours
            var refreshTokenHoursStr = Environment.GetEnvironmentVariable("REFRESH_TOKEN_LIFESPAN_HOURS");
            if (!int.TryParse(refreshTokenHoursStr, out var refreshTokenHours) || refreshTokenHours <= 0)
            {
                refreshTokenHours = 12;
            }

            this.refreshTokenLifeSpan = TimeSpan.FromHours(refreshTokenHours);
        }

        public TimeSpan AuthTokenLifeSpan => this.authTokenLifeSpan;

        public TimeSpan RefreshTokenLifeSpan => this.refreshTokenLifeSpan;

        public (string Token, DateTime Expiration) CreateToken(AppUser user)
        {
            var tokenExpiration = this.GetTokenExpiration(this.authTokenLifeSpan);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),  // ✅ REQUIRED for CurrentUserService
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty), // ✅ REQUIRED for email claims

                new Claim("UserId", user.Id.ToString()), // Custom claim for user ID
                new Claim("Email", user.Email ?? string.Empty), // Custom claim for user email
                new Claim("UserName", user.UserName ?? string.Empty), // Custom claim for first name
                new Claim("FirstName", user.FirstName ?? string.Empty), // Custom claim for first name
                new Claim("LastName", user.LastName ?? string.Empty), // Custom claim for last name
                new Claim("PhoneNumber", user.PhoneNumber ?? string.Empty), // Custom claim for phone number

                new Claim("TokenExpiration", tokenExpiration.ToString()),
            };

            // Add user name if available
            if (!string.IsNullOrEmpty(user.UserName))
            {
                claims.Add(new Claim(ClaimTypes.Name, user.UserName));     // ✅ REQUIRED for name claims
            }

            // Get user roles and add them as claims
            var roles = this.userManager.GetRolesAsync(user).Result;
            foreach (var role in roles) // ✅ ADD ALL ROLES
            {
                claims.Add(new Claim("UserRole", role)); // Custom claim for user role, will be filled later
                claims.Add(new Claim(ClaimTypes.Role, role));             // ✅ REQUIRED for authorization
            }

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(this.configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: this.configuration["Jwt:Issuer"],
                audience: this.configuration["Jwt:Audience"],
                claims: claims,
                expires: tokenExpiration,
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return (tokenString, tokenExpiration);
        }

        public DateTime GetTokenExpiration(TimeSpan lifeTime)
        {
            return DateTime.UtcNow.Add(lifeTime);
        }

        public RefreshToken CreateRefreshToken(AppUser user)
        {
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

            var refreshTokenExp = this.GetTokenExpiration(this.refreshTokenLifeSpan);

            var refreshTokenObj = new RefreshToken
            {
                Token = refreshToken,
                AppUserId = user.Id,
                Expires = refreshTokenExp,
            };

            return refreshTokenObj;
        }
    }
}
