using StoreManager.ViewModels.Order;

namespace StoreManager.Domain.IService
{
    public interface IOrderService
    {
        public Task<List<ProductDropDownItem>> GetProductDropDown();
        public Task<OrderVM> Get(int id);
        public Task<List<IndexOrderVM>> GetAll();
        public Task Delete(int id);
        public Task Insert(OrderVM order);
        public Task Update(OrderVM order);
    }
}
