namespace EventManagementSystem.Identity.Services
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using EventManagementSystem.Application.Interfaces;
    using EventManagementSystem.Domain.Models;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;

    public class TokenService : ITokenService
    {
        private readonly IConfiguration configuration;
        private readonly TimeSpan tokenLifeSpan;

        public TokenService(IConfiguration config)
        {
            this.configuration = config;
            this.tokenLifeSpan = TimeSpan.FromHours(1);
        }

        public string CreateToken(AppUser user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            };

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
