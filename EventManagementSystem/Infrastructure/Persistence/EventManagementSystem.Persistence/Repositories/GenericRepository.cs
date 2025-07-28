namespace EventManagementSystem.Persistence.Repositories
{
    using EventManagementSystem.Application.Interfaces;
    using EventManagementSystem.Persistence.Context;
    using Microsoft.EntityFrameworkCore;

    public class GenericRepository<T> : IRepository<T>
        where T : class
    {
        private readonly ApplicationDbContext context;
        private readonly DbSet<T> dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            this.context = context;
            this.dbSet = context.Set<T>();
        }

        public async Task<T> AddAsync(T entity)
        {
            await this.dbSet.AddAsync(entity);
            return entity;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await this.dbSet.ToListAsync();
        }

        public async Task<List<T>> GetAllWithIncludesAsync(params string[] includes)
        {
            IQueryable<T> query = this.dbSet;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }

        public async Task<T?> GetAsync(Guid id)
        {
            return await this.dbSet.FindAsync(id);
        }

        public async Task<T?> GetWithIncludesAsync(Guid id, params string[] includes)
        {
            IQueryable<T> query = this.dbSet;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);
        }

        public async Task<bool> RemoveAsync(Guid id)
        {
            var entity = await this.dbSet.FindAsync(id);
            if (entity == null)
            {
                return false;
            }

            this.dbSet.Remove(entity);
            return true;
        }

        public async Task<T?> UpdateAsync(T entity)
        {
            this.dbSet.Update(entity);
            return entity;
        }
    }
}
