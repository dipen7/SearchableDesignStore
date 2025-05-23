using StoreManager.Domain.Entities;

namespace StoreManager.Domain.IRepo
{
    public interface IUnitOfWork
    {
        IRepository<Product> Products { get; }
        IRepository<Supplier> Suppliers { get; }
        IRepository<Order> Orders { get; }
        IRepository<OrderDetail> OrderDetails { get; }

        Task<int> CompleteAsync();
        void RemoveTracks();
    }
}
