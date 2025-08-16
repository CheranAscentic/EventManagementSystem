namespace EventManagementSystem.Persistence.Repositories
{
    using EventManagementSystem.Application.Interfaces;
    using EventManagementSystem.Domain.Models;
    using EventManagementSystem.Persistence.Context;
    using Microsoft.EntityFrameworkCore;

    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext context;
        private readonly DbSet<RefreshToken> dbSet;

        public RefreshTokenRepository(ApplicationDbContext context)
        {
            this.context = context;
            this.dbSet = context.RefreshTokens;
        }

        public async Task<RefreshToken> AddAsync(RefreshToken entity)
        {
            await this.dbSet.AddAsync(entity);
            return entity;
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await this.dbSet.FindAsync(token);
        }

        public async Task<List<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId)
        {
            return await this.dbSet
                .Where(rt => rt.AppUserId == userId && rt.Revoked == null && rt.Expires > DateTime.UtcNow)
                .OrderByDescending(rt => rt.Created)
                .ToListAsync();
        }

        public async Task<List<RefreshToken>> GetAllTokensByUserIdAsync(Guid userId)
        {
            return await this.dbSet
                .Where(rt => rt.AppUserId == userId)
                .OrderByDescending(rt => rt.Created)
                .ToListAsync();
        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
            var refreshToken = await this.dbSet.FindAsync(token);
            if (refreshToken == null || refreshToken.IsRevoked)
            {
                return false;
            }

            refreshToken.Revoked = DateTime.UtcNow;
            this.dbSet.Update(refreshToken);
            return true;
        }

        public async Task<int> RevokeAllUserTokensAsync(Guid userId)
        {
            var activeTokens = await this.dbSet
                .Where(rt => rt.AppUserId == userId && rt.Revoked == null)
                .ToListAsync();

            foreach (var token in activeTokens)
            {
                token.Revoked = DateTime.UtcNow;
            }

            this.dbSet.UpdateRange(activeTokens);
            return activeTokens.Count;
        }

        public async Task<RefreshToken?> UpdateAsync(RefreshToken entity)
        {
            this.dbSet.Update(entity);
            return entity;
        }

        public async Task<bool> RemoveTokenAsync(string token)
        {
            var entity = await this.dbSet.FindAsync(token);
            if (entity == null)
            {
                return false;
            }

            this.dbSet.Remove(entity);
            return true;
        }

        public async Task<int> RemoveExpiredTokensAsync()
        {
            var expiredTokens = await this.dbSet
                .Where(rt => rt.Expires <= DateTime.UtcNow)
                .ToListAsync();

            this.dbSet.RemoveRange(expiredTokens);
            return expiredTokens.Count;
        }
    }
}