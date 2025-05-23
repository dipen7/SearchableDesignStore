using StoreManager.Domain.Entities;

namespace StoreManager.Domain.IRepo
{
    public interface IOrderDetailRepo
    {
        public Task<OrderDetail> Get(int id);
        public Task<List<Supplier>> GetList(int[] ids);
        public Task DeleteList(int[] ids);
        public Task InsertList(List<OrderDetail> orderDetails);
    }
}
