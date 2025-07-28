namespace EventManagementSystem.Persistence.Repositories
{

    using EventManagementSystem.Application.Interfaces;
    using EventManagementSystem.Domain.Models;
    using EventManagementSystem.Persistence.Context;

    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext context;

        public IRepository<Event> Events { get; }

        public IRepository<EventRegistration> EventRegistrations { get; }

        public IRepository<EventImage> EventImages { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            this.context = context;
            this.Events = new GenericRepository<Event>(context);
            this.EventRegistrations = new GenericRepository<EventRegistration>(context);
            this.EventImages = new GenericRepository<EventImage>(context);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await this.context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Releases all resources used by the <see cref="UnitOfWork"/> instance.
        /// </summary>
        /// <remarks>
        /// This method disposes the underlying <see cref="ApplicationDbContext"/>, which manages the database connection
        /// and tracks changes to entities. Properly disposing the context is essential to free up database connections
        /// and unmanaged resources, preventing memory leaks and connection pool exhaustion.
        /// <para>
        /// In line with SOLID and Clean Architecture principles, resource management is handled at the infrastructure layer,
        /// keeping business logic decoupled from data access concerns. When using dependency injection in ASP.NET Core,
        /// the DI container will automatically call <c>Dispose</c> at the end of the request lifetime.
        /// </para>
        /// <para>
        /// Always ensure <c>Dispose</c> is called when the <see cref="UnitOfWork"/> is no longer needed, especially in
        /// manual or non-DI scenarios (e.g., background services, console apps).
        /// </para>
        /// </remarks>
        public void Dispose()
        {
            context.Dispose();
        }
    }
}