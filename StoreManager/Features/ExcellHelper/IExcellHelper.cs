using StoreManager.Features.ExcellHelper.Model;
using StoreManager.ViewModels.Product;

namespace StoreManager.Features.ExcellHelper
{
    public interface IExcellHelper
    {
        ExcelRecords<ProductVM> ReadProductsFromExcel(IFormFile file);
        ExcelRecords<ProductVM> ReadProductsFromCSV(IFormFile file);
    }
}
