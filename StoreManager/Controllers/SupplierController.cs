using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreManager.Domain.IService;
using StoreManager.ViewModels.Supplier;

namespace StoreManager.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SupplierController : Controller
    {
        private readonly ILogger<SupplierController> _logger;
        private readonly ISupplierService _supplierService;
        public SupplierController(ILogger<SupplierController> logger, ISupplierService supplierService)
        {
            _logger = logger;
            _supplierService = supplierService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<JsonResult> GetSuppliers()
        {
            try
            {
                var suppliers = await _supplierService.GetAll();
                return Json(new { data = suppliers, error = false, message=""});

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Json(new { data = new List<SupplierVM>(), error = true, message = ex.Message });
            
            }

        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupplierVM supplier)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _supplierService.Insert(supplier);

                    TempData["SuccessMessage"] = "Supplier created successfully!";
                    return RedirectToAction("Index");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                ModelState.AddModelError("", ex.Message);

            }
            return View(supplier);

        }

        public async Task<ActionResult> Edit(int id)
        {
            var supplier = await _supplierService.Get(id);
            if(supplier == null)
            {
                TempData["ErrorMessage"] = "Data Not Found!";
                return RedirectToAction("Index");

            }
            return View(supplier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(SupplierVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _supplierService.Update(model);

                    TempData["SuccessMessage"] = "Supplier Edited successfully!";
                    return RedirectToAction("Index");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                ModelState.AddModelError("", ex.Message);

            }
            return View(model);
        }

        public async Task<ActionResult> Details(int id)
        {
            var supplier = await _supplierService.Get(id);
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
            await _supplierService.Delete(id);

            return Json(new { success = true });
        }

    }
}
