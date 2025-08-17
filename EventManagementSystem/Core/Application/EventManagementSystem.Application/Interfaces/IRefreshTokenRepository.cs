namespace EventManagementSystem.Application.Interfaces
{
    using EventManagementSystem.Domain.Models;

    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> AddAsync(RefreshToken entity);

        Task<RefreshToken?> GetByTokenAsync(string token);

        Task<List<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId);

        Task<List<RefreshToken>> GetAllTokensByUserIdAsync(Guid userId);

        Task<bool> RevokeTokenAsync(string token);

        Task<int> RevokeAllUserTokensAsync(Guid userId);

        Task<RefreshToken?> UpdateAsync(RefreshToken entity);

        Task<bool> RemoveTokenAsync(string token);

        Task<int> RemoveExpiredTokensAsync();

        Task<int> RemoveAllUserTokensAsync(Guid userId);
    }
}