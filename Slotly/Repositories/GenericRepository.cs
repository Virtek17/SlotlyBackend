using Microsoft.EntityFrameworkCore;
using Slotly.Data;
using Slotly.Interfaces;
using System.Linq.Expressions;

namespace Slotly.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly SlotlyContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(SlotlyContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>(); 
        }

        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task<T?> GetByIdAsync(Guid id) => await _dbSet.FindAsync(id);

        public void Delete(T entity) => _dbSet.Remove(entity);

        public void Update(T entity) => _dbSet.Update(entity);

        public async Task SaveAsync() => await _context.SaveChangesAsync();

        public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            // Динамически добавляем все Include, которые передадим
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }

    }
}
