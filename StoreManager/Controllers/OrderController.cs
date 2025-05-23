using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreManager.Domain.Entities;
using StoreManager.Domain.IService;
using StoreManager.ViewModels.Order;
using System.Text.Json;

namespace StoreManager.Controllers
{
    [Authorize(Roles = "Admin,Member")]
    public class OrderController : Controller
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IOrderService _orderService;
        public OrderController(ILogger<OrderController> logger, IOrderService orderService)
        {
            _logger = logger;
            _orderService = orderService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<JsonResult> GetOrders()
        {
            var products = await _orderService.GetAll();

            return Json(new { data = products });
        }
        public async Task<IActionResult> Create()
        {
            var productList = await _orderService.GetProductDropDown();
            ViewBag.ProductsJson = JsonSerializer.Serialize(productList);

            OrderVM model = new OrderVM();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderVM order)
        {
            var productList = await _orderService.GetProductDropDown();
            try
            {
                if (order.OrderDetails == null)
                {
                    ModelState.AddModelError("OrderDetails", "Atleast one product is needed.");
                    order.OrderDetails = new List<OrderDetailVM>();

                    ViewBag.ProductsJson = JsonSerializer.Serialize(productList);
                    return View(order);
                }
                if (order.OrderDetails.Count == 0)
                {
                    ModelState.AddModelError("OrderDetails", "Atleast one product is needed.");

                    ViewBag.ProductsJson = JsonSerializer.Serialize(productList);
                    return View(order);

                }
                if (ModelState.IsValid)
                {
                    try
                    {
                        await _orderService.Insert(order);

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                        ModelState.AddModelError("", ex.Message);

                    }

                    TempData["SuccessMessage"] = "order created successfully!";
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "All Fields are needed.");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                ModelState.AddModelError("", ex.Message);

            }

            ViewBag.ProductsJson = JsonSerializer.Serialize(productList);
            return View(order);
        }

        public async Task<IActionResult> Edit(int id)
        {

            var productList = await _orderService.GetProductDropDown(); 
            ViewBag.ProductsJson = JsonSerializer.Serialize(productList);

            var order = await _orderService.Get(id);
            if (order == null)
            {
                TempData["ErrorMessage"] = "Data Not Found!";
                return RedirectToAction("Index");
            }

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OrderVM model)
        {

            var productList = await _orderService.GetProductDropDown();
            ViewBag.ProductsJson = JsonSerializer.Serialize(productList);
            if (model.OrderDetails == null)
            {
                model.OrderDetails = new List<OrderDetailVM>();
                ModelState.AddModelError("", "Atleast one product is needed.");

                return View(model);
            }
            if (model.OrderDetails.Count == 0)
            {
                ModelState.AddModelError("", "Atleast one product is needed.");

                return View(model);

            }

            try
            {
                await _orderService.Update(model);

                TempData["SuccessMessage"] = "Order Edited successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                ModelState.AddModelError("", ex.Message);

            }
            if (model.OrderDetails == null)
            {
                model.OrderDetails = new List<OrderDetailVM>();
            }
            return View(model);
        }


    }
}
