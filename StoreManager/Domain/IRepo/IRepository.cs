using System.Linq.Expressions;

namespace StoreManager.Domain.IRepo
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task BulkAddAsync(List<T> entities);
        Task<List<T>> GetByFilter(Expression<Func<T, bool>> filter = null);
        Task<bool> CheckIfExists(Expression<Func<T, bool>> filter = null);
    }
}
