using StoreManager.Domain.Entities;
using StoreManager.ViewModels.Product;
using StoreManager.ViewModels.Product.Excel;

namespace StoreManager.Domain.IService
{
    public interface IProductService
    {
        public Task<ProductWithImage> Get(int id);
        public Task<List<ProductVM>> GetAll();
        public Task Delete(int id);
        public Task<ProductWithImageResult> InsertWithImage(ProductWithImage product);
        public Task<ExcelResult> ExcelUpload(IFormFile uploadedFile);
        public Task<ExcelResult> CsvUpload(IFormFile uploadedFile);
        public Task<ProductWithImageResult> Update(ProductWithImage product);

    }
}
