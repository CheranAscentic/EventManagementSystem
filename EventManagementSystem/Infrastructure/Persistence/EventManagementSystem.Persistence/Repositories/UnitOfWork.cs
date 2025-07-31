namespace EventManagementSystem.Persistence.Repositories
{
    using EventManagementSystem.Application.Interfaces;
    using EventManagementSystem.Domain.Models;
    using EventManagementSystem.Persistence.Context;
    using Microsoft.EntityFrameworkCore.Storage;
    using System;

    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext context;
        private IDbContextTransaction? currentTransaction;

        public UnitOfWork(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await this.context.SaveChangesAsync(cancellationToken);
        }

        [Obsolete("Deprecated: SaveChangesAsync automatically handles transactions.")]
        public async Task BeginTransactionAsync()
        {
            if (this.currentTransaction == null)
            {
                this.currentTransaction = await this.context.Database.BeginTransactionAsync();
            }
        }

        [Obsolete("Deprecated: SaveChangesAsync automatically handles transactions.")]
        public async Task CommitTransactionAsync()
        {
            if (this.currentTransaction != null)
            {
                await this.currentTransaction.CommitAsync();
                await this.currentTransaction.DisposeAsync();
                this.currentTransaction = null;
            }
        }

        [Obsolete("Deprecated: SaveChangesAsync automatically handles transactions.")]
        public async Task RollbackTransactionAsync()
        {
            if (this.currentTransaction != null)
            {
                await this.currentTransaction.RollbackAsync();
                await this.currentTransaction.DisposeAsync();
                this.currentTransaction = null;
            }
        }

        [Obsolete("Deprecated: SaveChangesAsync automatically handles transactions.")]
        public void Dispose()
        {
            this.context.Dispose();
        }
    }
}