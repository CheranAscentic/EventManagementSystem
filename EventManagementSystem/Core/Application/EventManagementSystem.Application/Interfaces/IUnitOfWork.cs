using System;
using System.Threading;
using System.Threading.Tasks;
using EventManagementSystem.Domain.Models;

namespace EventManagementSystem.Application.Interfaces
{
    /// <summary>
    /// Represents a unit of work that coordinates the work of multiple repositories by managing
    /// transactions and saving changes as a single atomic operation.
    /// </summary>
    /// <remarks>
    /// The IUnitOfWork interface is a key part of the repository and unit of work patterns.
    /// It ensures that all changes made through the repositories are committed together,
    /// providing transactional consistency and making it possible to roll back changes if needed.
    /// <para>
    /// Usage:
    /// <list type="number">
    /// <item>
    /// <description>Begin a transaction with <see cref="BeginTransactionAsync"/> if you need explicit transaction control.</description>
    /// </item>
    /// <item>
    /// <description>Perform operations using the exposed repositories (e.g., <see cref="IRepository{T}"/>).</description>
    /// </item>
    /// <item>
    /// <description>Call <see cref="SaveChangesAsync"/> to persist all changes as a single unit.</description>
    /// </item>
    /// <item>
    /// <description>Commit the transaction with <see cref="CommitTransactionAsync"/> or roll back with <see cref="RollbackTransactionAsync"/> as needed.</description>
    /// </item>
    /// </list>
    /// <para>
    /// Always dispose the unit of work when done to release resources. In ASP.NET Core, this is handled automatically by the DI container.
    /// </para>
    /// </remarks>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Gets the repository for <see cref="Event"/> entities.
        /// </summary>
        IRepository<Event> Events { get; }

        /// <summary>
        /// Gets the repository for <see cref="EventRegistration"/> entities.
        /// </summary>
        IRepository<EventRegistration> EventRegistrations { get; }

        /// <summary>
        /// Gets the repository for <see cref="EventImage"/> entities.
        /// </summary>
        IRepository<EventImage> EventImages { get; }

        /// <summary>
        /// Saves all changes made in this unit of work to the database as a single transaction.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The number of state entries written to the database.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Begins a new database transaction. Use this when you need to group multiple operations into a single transaction.
        /// </summary>
        Task BeginTransactionAsync();

        /// <summary>
        /// Commits the current transaction, making all changes permanent.
        /// </summary>
        Task CommitTransactionAsync();

        /// <summary>
        /// Rolls back the current transaction, undoing all changes made during the transaction.
        /// </summary>
        Task RollbackTransactionAsync();
    }
}
