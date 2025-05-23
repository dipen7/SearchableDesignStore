using Microsoft.EntityFrameworkCore;
using StoreManager.Domain.IRepo;
using EFCore.BulkExtensions;
using System.Linq.Expressions;
using StoreManager.Domain.Entities;

namespace StoreManager.Infrastructure.Data.Repo
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual async Task BulkAddAsync(List<T> entities)
        {
            await _context.BulkInsertAsync(entities);
        }

        public async Task<List<T>> GetByFilter(Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }
            return query.ToList();
        }

        public async Task<bool> CheckIfExists(Expression<Func<T, bool>> filter = null)
        {
            return await _dbSet.AnyAsync(filter);
        }
    }
}
