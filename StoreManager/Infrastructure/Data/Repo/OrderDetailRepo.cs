using StoreManager.Domain.Entities;
using StoreManager.Domain.IRepo;

namespace StoreManager.Infrastructure.Data.Repo
{
    public class OrderDetailRepo : IOrderDetailRepo
    {
        public Task DeleteList(int[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<OrderDetail> Get(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Supplier>> GetList(int[] ids)
        {
            throw new NotImplementedException();
        }

        public Task InsertList(List<OrderDetail> orderDetails)
        {
            throw new NotImplementedException();
        }
    }
}
