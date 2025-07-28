namespace EventManagementSystem.Application.Interfaces
{
    public interface IRepository<T>
        where T : class
    {
        Task<T> AddAsync(T entity);

        Task<T?> GetAsync(Guid id);

        Task<T?> GetWithIncludesAsync(Guid id, params string[] includes);

        Task<List<T>> GetAllAsync();

        Task<List<T>> GetAllWithIncludesAsync(params string[] includes);

        Task<bool> RemoveAsync(Guid id);

        Task<T?> UpdateAsync(T entity);
    }
}
