using StoreManager.Domain.Entities;
using StoreManager.Domain.IRepo;
using StoreManager.Domain.IService;
using StoreManager.ViewModels.Supplier;

namespace StoreManager.Infrastructure.Service
{
    public class SupplierService : ISupplierService
    {
        private readonly ILogger<ISupplierService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public SupplierService(IUnitOfWork unitOfWork, ILogger<ISupplierService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Delete(int id)
        {
            var dbObject = await _unitOfWork.Suppliers.GetByIdAsync(id);
            if (dbObject == null)
                throw new Exception("Object not found.");

            _unitOfWork.Suppliers.Delete(dbObject);
            await _unitOfWork.CompleteAsync();

        }

        public async Task<SupplierVM?> Get(int id)
        {
            Supplier? dbObject = await _unitOfWork.Suppliers.GetByIdAsync(id);
            if(dbObject == null)
            {
                return null;
            }
            else
            {
                return new SupplierVM()
                {
                    Address = dbObject.Address,
                    City = dbObject.City,
                    ContactNumber = dbObject.ContactNumber,
                    Country = dbObject.Country,
                    CreatedDate = dbObject.CreatedDate,
                    Email = dbObject.Email,
                    SupplierID = dbObject.SupplierID,
                    SupplierName= dbObject.SupplierName
                };
            }
        }

        public async Task<List<SupplierVM>> GetAll()
        {
            return (await _unitOfWork.Suppliers.GetAllAsync()).Select(x => new SupplierVM
            {
                Address = x.Address,
                City = x.City,
                ContactNumber = x.ContactNumber,
                Country = x.Country,
                CreatedDate = x.CreatedDate,
                Email = x.Email,
                SupplierID = x.SupplierID,
                SupplierName = x.SupplierName
            }).ToList();
        }

        public async Task Insert(SupplierVM supplier)
        {
            if (await _unitOfWork.Suppliers.CheckIfExists(dbS => dbS.Email == supplier.Email))
            {
                throw new Exception("Supplier Email already exists.");
            }
            else
            {
                Supplier entity = new Supplier()
                {
                    Address = supplier.Address,
                    City = supplier.City,
                    ContactNumber = supplier.ContactNumber,
                    Country = supplier.Country,
                    CreatedDate = DateTime.Now,
                    Email = supplier.Email,
                    SupplierName = supplier.SupplierName
                };
                try
                {
                    await _unitOfWork.Suppliers.AddAsync(entity);
                    await _unitOfWork.CompleteAsync();

                }
                catch(Exception ex)
                {
                    throw;
                }
            }
        }

        public async Task Update(SupplierVM supplier)
        {
            Supplier? dbObject = await _unitOfWork.Suppliers.GetByIdAsync(supplier.SupplierID);
            if (dbObject == null)
            {
                throw new Exception("Data Not Found!");
            }
            if (dbObject.Email != supplier.Email)
            {
                if (await _unitOfWork.Suppliers.CheckIfExists(dbS => dbS.Email == supplier.Email))
                {
                    throw new Exception("Supplier Email already exists.");
                }
            }

            dbObject.Address = supplier.Address;
            dbObject.City = supplier.City;
            dbObject.ContactNumber = supplier.ContactNumber;
            dbObject.Country = supplier.Country;
            dbObject.CreatedDate = DateTime.Now;
            dbObject.Email = supplier.Email;
            dbObject.SupplierName = supplier.SupplierName;

            _unitOfWork.Suppliers.Update(dbObject);
            await _unitOfWork.CompleteAsync();

        }
    }
}
