using System;
using System.Threading;
using System.Threading.Tasks;
using EventManagementSystem.Domain.Models;

namespace EventManagementSystem.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Event> Events { get; }

        IRepository<EventRegistration> EventRegistrations { get; }

        IRepository<EventImage> EventImages { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}