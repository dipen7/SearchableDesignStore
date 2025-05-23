using StoreManager.ViewModels.Supplier;

namespace StoreManager.Domain.IService
{
    public interface ISupplierService
    {
        public Task<SupplierVM?> Get(int id);
        public Task<List<SupplierVM>> GetAll();
        public Task Delete(int id);
        public Task Insert(SupplierVM supplier);
        public Task Update(SupplierVM supplier);
    }
}
