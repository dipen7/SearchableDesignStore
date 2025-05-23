using Microsoft.EntityFrameworkCore;
using StoreManager.Domain.Entities;
using StoreManager.Domain.IRepo;

namespace StoreManager.Infrastructure.Data.Repo
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        private IRepository<Product>? _products;
        private IRepository<Supplier>? _suppliers;
        private IRepository<Order>? _orders;
        private IRepository<OrderDetail>? _orderDetails;

        public IRepository<Product> Products => _products ??= new Repository<Product>(_context);
        public IRepository<Supplier> Suppliers => _suppliers ??= new Repository<Supplier>(_context);
        public IRepository<Order> Orders => _orders ??= new Repository<Order>(_context);
        public IRepository<OrderDetail> OrderDetails => _orderDetails ??= new Repository<OrderDetail>(_context);

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void RemoveTracks()
        {
            foreach (var entry in _context.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.CurrentValues.SetValues(entry.OriginalValues);
                        entry.State = EntityState.Unchanged;
                        break;

                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Unchanged;
                        break;
                }
            }
        }
    }
}
