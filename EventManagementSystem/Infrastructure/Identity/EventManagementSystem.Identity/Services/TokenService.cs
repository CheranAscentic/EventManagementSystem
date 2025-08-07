namespace EventManagementSystem.Identity.Services
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using EventManagementSystem.Application.Interfaces;
    using EventManagementSystem.Domain.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;

    public class TokenService : ITokenService
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<AppUser> userManager;
        private readonly TimeSpan tokenLifeSpan;

        public TokenService(IConfiguration config, UserManager<AppUser> userManager)
        {
            this.configuration = config;
            this.userManager = userManager;
            this.tokenLifeSpan = TimeSpan.FromHours(3);
        }

        public string CreateToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),  // ✅ REQUIRED for CurrentUserService
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),   // ✅ REQUIRED for email claims
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
                claims.Add(new Claim(ClaimTypes.Role, role));             // ✅ REQUIRED for authorization
            }

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(this.configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: this.configuration["Jwt:Issuer"],
                audience: this.configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.Add(this.tokenLifeSpan),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public DateTime GetTokenExpiration()
        {
            return DateTime.UtcNow.Add(this.tokenLifeSpan);
        }
    }
}
