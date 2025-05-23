using Microsoft.AspNetCore.Hosting;
using StoreManager.Domain.Entities;
using StoreManager.Domain.IRepo;
using StoreManager.Domain.IService;
using StoreManager.Features.ExcellHelper;
using StoreManager.Features.ImageHelper;
using StoreManager.ViewModels.Product;
using StoreManager.ViewModels.Product.Excel;

namespace StoreManager.Infrastructure.Service
{
    public class ProductService : IProductService
    {
        private readonly ILogger<IProductService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IExcellHelper _excellHelper;
        private readonly string _productImagePath;
        private readonly string _productImageFolder;
        private readonly IImageHelper _imageHelper;
        public ProductService(IUnitOfWork unitOfWork, ILogger<IProductService> logger, IExcellHelper excellHelper, IWebHostEnvironment webHostEnvironment, IImageHelper imageHelper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _excellHelper = excellHelper;
            _imageHelper = imageHelper;
            _productImageFolder = "uploads";
            _productImagePath = Path.Combine(webHostEnvironment.WebRootPath, _productImageFolder);
            Directory.CreateDirectory(_productImagePath); // Ensure folder exists

        }

        public async Task BulkInsert(List<ProductVM> products)
        {
            var trackedSupplierEmails = (await _unitOfWork.Suppliers.GetAllAsync()).Select(x => x.Email);
            var untrackedSupplierEmails = products.Select(p => p.SupplierEmail).Except(trackedSupplierEmails, StringComparer.OrdinalIgnoreCase).ToList();
            try
            {
                if (untrackedSupplierEmails.Count > 0) 
                { 
                    await _unitOfWork.Suppliers.BulkAddAsync(untrackedSupplierEmails.Select(e=>getNewSupplier(e)).ToList());

                }
                var productEntities = products.Select(product => new Product() 
                {
                    Category = product.Category,
                    Description = product.Description,
                    ExpiryDate = product.ExpiryDate,
                    IsActive = product.IsActive,
                    ManufacturedDate = product.ManufacturedDate,
                    Price = product.Price,
                    ProductName = product.ProductName,
                    StockQuantity = product.StockQuantity,
                    SupplierEmail = product.SupplierEmail,
                    CreatedDate = DateTime.Now,
                    imageUrl = ""
                }).ToList();
                await _unitOfWork.Products.BulkAddAsync(productEntities);
                await _unitOfWork.CompleteAsync();

            }
            catch (Exception ex)
            {
                _unitOfWork.RemoveTracks();
                throw;

            }
        }

        public async Task<ExcelResult> CsvUpload(IFormFile uploadedFile)
        {
            ExcelResult result = new ExcelResult();
            var fileProducts = _excellHelper.ReadProductsFromCSV(uploadedFile);
            result.FailedIndices = fileProducts.FailedIndices;
            if (fileProducts.ReadError)
            {
                result.ReadError = fileProducts.ReadError;
            }
            try 
            {
                await BulkInsert(fileProducts.Records);
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex.Message);
                result.DbError = true;
            }
            return result;

        }

        public async Task Delete(int id)
        {
            var dbObject = await _unitOfWork.Products.GetByIdAsync(id);
            if (dbObject == null)
                throw new Exception("Object not found.");
            
            var isLinkedDbObject = await _unitOfWork.OrderDetails.CheckIfExists(c => c.ProductID == id);
            if (isLinkedDbObject)
                throw new Exception("Product is in use in order so can not be deleted.");
            
            _unitOfWork.Products.Delete(dbObject);
            await _unitOfWork.CompleteAsync();

            try
            {
                if (!string.IsNullOrEmpty(dbObject.imageUrl))
                    _imageHelper.DeleteFile(Path.Combine(_productImagePath, dbObject.imageUrl.Remove(0, 8)));

            }
            catch (Exception ex)
            {

            }

        }

        public async Task<ExcelResult> ExcelUpload(IFormFile uploadedFile)
        {
            ExcelResult result = new ExcelResult();
            var fileProducts = _excellHelper.ReadProductsFromExcel(uploadedFile);
            result.FailedIndices = fileProducts.FailedIndices;
            if (fileProducts.ReadError)
            {
                result.ReadError = fileProducts.ReadError;
            }
            try
            {
                await BulkInsert(fileProducts.Records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                result.DbError = true;
            }
            return result;

        }

        public async Task<ProductWithImage?> Get(int id)
        {
            try
            {
                Product? dbObject = await _unitOfWork.Products.GetByIdAsync(id);
                if (dbObject == null)
                {
                    return null;
                }
                else
                {
                    return new ProductWithImage()
                    {
                        Category = dbObject.Category,
                        Description = dbObject.Description,
                        ExpiryDate = dbObject.ExpiryDate,
                        IsActive = dbObject.IsActive,
                        ManufacturedDate = dbObject.ManufacturedDate,
                        Price = dbObject.Price,
                        ProductId = dbObject.ProductId,
                        ProductName = dbObject.ProductName,
                        StockQuantity = dbObject.StockQuantity,
                        CreatedDate = dbObject.CreatedDate,
                        ImageUrl = dbObject.imageUrl,
                        SupplierEmail = dbObject.SupplierEmail
                    };
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;

            }

        }

        public async Task<List<ProductVM>> GetAll()
        {
            var result = from product in await _unitOfWork.Products.GetAllAsync()
                         join supplier in await _unitOfWork.Suppliers.GetAllAsync()
                         on product.SupplierEmail equals supplier.Email
                         into productSuppliers // Group join
                         from supplier in productSuppliers.DefaultIfEmpty() // Left join behavior
                         select new ProductVM
                         {
                             Category = product.Category,
                             SupplierEmail = supplier?.Email ?? "",
                             SupplierID = product.SupplierID,
                             CreatedDate = product.CreatedDate,
                             Description = product.Description,
                             ExpiryDate = product.ExpiryDate,
                             IsActive = product.IsActive,
                             ManufacturedDate = product.ManufacturedDate,
                             Price = product.Price,
                             ProductId = product.ProductId,
                             ProductName = product.ProductName,
                             StockQuantity = product.StockQuantity
                         };
            return result.ToList();
        }

        private async Task Insert(ProductWithImage product)
        {
            if (!await _unitOfWork.Suppliers.CheckIfExists(dbS => dbS.Email == product.SupplierEmail))
            {
                await _unitOfWork.Suppliers.AddAsync(getNewSupplier(product.SupplierEmail) );
            }
            Product entity = new Product()
            {
                Category = product.Category,
                Description = product.Description,
                ExpiryDate = product.ExpiryDate,
                IsActive = product.IsActive,
                ManufacturedDate=product.ManufacturedDate,
                Price= product.Price,
                ProductName= product.ProductName,
                StockQuantity = product.StockQuantity,
                SupplierEmail = product.SupplierEmail,
                CreatedDate = DateTime.Now,
                imageUrl = product.ImageUrl
            };
            try
            {
                await _unitOfWork.Products.AddAsync(entity);
                await _unitOfWork.CompleteAsync();

            }
            catch (Exception ex)
            {
                _unitOfWork.RemoveTracks();
                throw;
            }
        }

        public async Task<ProductWithImageResult> InsertWithImage(ProductWithImage product)
        {
            ProductWithImageResult result = new ProductWithImageResult();
            try
            {
                product.ImageUrl = await _imageHelper.UploadFIle(product.ProductImage, _productImagePath, _productImageFolder);
                result.IsImageSuccess = true;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                result.IsImageSuccess = false;

            }
            try
            {
                await Insert(product);
                result.IsProductSuccess = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                result.IsProductSuccess = false;

            }


            return result;
        }

        public async Task<ProductWithImageResult> Update(ProductWithImage product)
        {
            Product? dbObject = await _unitOfWork.Products.GetByIdAsync(product.ProductId);
            if (dbObject == null)
            {
                throw new Exception("Data Not Found!");
            }

            ProductWithImageResult result = new ProductWithImageResult();
            try
            {
                if (product.ProductImage != null && product.ProductImage.Length > 0)
                {
                    if (!string.IsNullOrWhiteSpace(product.ImageUrl))
                    {
                        string oldImage = product.ImageUrl?.Remove(0, 8) ?? "";
                        _imageHelper.DeleteFile(Path.Combine(_productImagePath, oldImage));

                    }

                    product.ImageUrl = await _imageHelper.UploadFIle(product.ProductImage, _productImagePath, _productImageFolder);
                }
                result.IsImageSuccess = true;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                result.IsImageSuccess = false;

            }

            if (!await _unitOfWork.Suppliers.CheckIfExists(dbS => dbS.Email == product.SupplierEmail))
            {
                await _unitOfWork.Suppliers.AddAsync(getNewSupplier(product.SupplierEmail));
            }

            dbObject.Category = product.Category;
            dbObject.Description = product.Description;
            dbObject.ExpiryDate = product.ExpiryDate;
            dbObject.IsActive = product.IsActive;
            dbObject.ManufacturedDate = product.ManufacturedDate;
            dbObject.Price = product.Price;
            dbObject.ProductName = product.ProductName;
            dbObject.StockQuantity = product.StockQuantity;
            dbObject.SupplierEmail = product.SupplierEmail;
            dbObject.imageUrl = product.ImageUrl??"";

            try
            {
                _unitOfWork.Products.Update(dbObject);
                await _unitOfWork.CompleteAsync();
                result.IsProductSuccess = true;

            }
            catch (Exception ex)
            {
                _unitOfWork.RemoveTracks();
                result.IsProductSuccess = false;
                throw;
            }
            return result;
        }

        private Supplier getNewSupplier(string email)
        {
            return new Supplier()
            {
                Email = email,
                Address = "",
                City = "",
                ContactNumber = "",
                Country = "",
                CreatedDate = DateTime.Now,
                SupplierName = ""
            };
        }
    }
}
