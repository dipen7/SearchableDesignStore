using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreManager.Domain.IService;
using StoreManager.ViewModels.Product;
using StoreManager.ViewModels.Product.Excel;

namespace StoreManager.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductService _productService;
        public ProductController(ILogger<ProductController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<JsonResult> GetProducts()
        {
            try
            {
                var products = await _productService.GetAll();
                return Json(new { data = products, error = false, message = "" });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Json(new { data = new List<ProductVM>(), error = true, message = ex.Message });

            }

        }
        public IActionResult Create()
        {
            ProductWithImage model = new ProductWithImage();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductWithImage product)
        {
            try
            {
                if (product.ProductImage != null && product.ProductImage.Length > 0)
                {
                    var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                    if (!allowedTypes.Contains(product.ProductImage.ContentType))
                    {
                        ModelState.AddModelError("ProductImage", "Only image files (JPG, PNG, GIF) are allowed.");
                        return View(product);
                    }
                    if (product.ProductImage.Length > 4 * 1024 * 1024) // 4 MB
                    {
                        ModelState.AddModelError("ProductImage", "File size must be less than 4 MB.");
                        return View(product);
                    }
                }
                ModelState.Remove("ImageUrl");
                if (ModelState.IsValid)
                {
                    var result = await _productService.InsertWithImage(product);

                    if (!result.IsImageSuccess)
                    {
                        TempData["ErrorMessage"] = "Product image was not uploaded!";

                    }
                    if (!result.IsProductSuccess)
                    {
                        TempData["ErrorMessage"] = "some error occured";

                    }
                    if (result.IsProductSuccess && result.IsImageSuccess)
                    {
                        TempData["SuccessMessage"] = "Product created successfully!";

                    }
                    return RedirectToAction("Index");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                ModelState.AddModelError("", ex.Message);

            }
            return View(product);
        }

        public async Task<ActionResult> Edit(int id)
        {
            var supplier = await _productService.Get(id);
            if (supplier == null)
            {
                TempData["ErrorMessage"] = "Data Not Found!";
                return RedirectToAction("Index");

            }
            return View(supplier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ProductWithImage product)
        {
            try
            {
                if (product.ProductImage != null && product.ProductImage.Length > 0)
                {
                    var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                    if (!allowedTypes.Contains(product.ProductImage.ContentType))
                    {
                        ModelState.AddModelError("ProductImage", "Only image files (JPG, PNG, GIF) are allowed.");
                        return View(product);
                    }
                    if (product.ProductImage.Length > 4 * 1024 * 1024) // 4 MB
                    {
                        ModelState.AddModelError("ProductImage", "File size must be less than 4 MB.");
                        return View(product);
                    }
                }
                ModelState.Remove("ImageUrl");
                ModelState.Remove("ProductImage");
                
                if (ModelState.IsValid)
                {
                    ProductWithImageResult result = new ProductWithImageResult();
                    result = await _productService.Update(product);

                    if (!result.IsImageSuccess)
                    {
                        TempData["ErrorMessage"] = "Product image was not uploaded!";

                    }
                    if (!result.IsProductSuccess)
                    {
                        TempData["ErrorMessage"] = "some error occured";

                    }
                    if (result.IsProductSuccess && result.IsImageSuccess)
                    {
                        TempData["SuccessMessage"] = "Product updated successfully!";

                    }
                    return RedirectToAction("Index");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                ModelState.AddModelError("", ex.Message);

            }
            return View(product);
        }

        public async Task<ActionResult> Details(int id)
        {
            var supplier = await _productService.Get(id);
            if (supplier == null)
            {
                TempData["ErrorMessage"] = "Data Not Found!";
                return RedirectToAction("Index");

            }
            return View(supplier);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _productService.Delete(id);

            }
            catch(Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = true });
        }
        public IActionResult Upload()
        {
            ExcelResult result = new ExcelResult();
            return View(result);
        }
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile excelFile)
        {
            ExcelResult result = new ExcelResult();
            if (excelFile == null || excelFile.Length == 0)
            {
                ModelState.AddModelError("", "Please upload a file.");
                return View(result);
            }

            var extension = Path.GetExtension(excelFile.FileName).ToLower();
            var allowedExtensions = new[] { ".xlsx", ".xls", ".csv" };

            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("", "Only .xlsx, .xls, or .csv files are allowed.");
                return View(result);
            }
            // Handle Excel files
            if (extension == ".xlsx" || extension == ".xls")
            {
                result = await _productService.ExcelUpload(excelFile);
            }

            // Handle CSV files
            else if (extension == ".csv")
            {
                result = await _productService.CsvUpload(excelFile);
            }

            return View(result);
        }
    }
}
