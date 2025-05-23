using StoreManager.Domain.Entities;
using StoreManager.Domain.IRepo;
using StoreManager.Domain.IService;
using StoreManager.ViewModels.Order;

namespace StoreManager.Infrastructure.Service
{
    public class OrderService : IOrderService
    {
        private readonly ILogger<IOrderService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public OrderService(IUnitOfWork unitOfWork, ILogger<IOrderService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;

        }

        public async Task<List<ProductDropDownItem>> GetProductDropDown()
        {
            try
            {
                var dbprodlist = (await _unitOfWork.Products.GetAllAsync()).Select(prod =>
                new ProductDropDownItem()
                {
                    ProductID = prod.ProductId,
                    Price = prod.Price,
                    ProductName = prod.ProductName,
                    StockQuantity = prod.StockQuantity
                }).ToList();
                return dbprodlist;

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error in OrderService.GetProductDropDown");
                return new List<ProductDropDownItem>();
            }
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<OrderVM> Get(int id)
        {
            var result = (from order in await _unitOfWork.Orders.GetAllAsync()
                          join detail in await _unitOfWork.OrderDetails.GetAllAsync() on order.OrderID equals detail.OrderID
                          join product in await _unitOfWork.Products.GetAllAsync() on detail.ProductID equals product.ProductId
                          where order.OrderID == id
                          select new
                          {
                              OrderID = order.OrderID,
                              OrderDate = order.OrderDate,
                              CustomerName = order.CustomerName,
                              CustomerEmail = order.CustomerEmail,
                              TotalAmount = order.TotalAmount,
                              OrderStatus = order.OrderStatus,
                              OrderCreatedDate = order.CreatedDate,

                              OrderDetailID = detail.OrderDetailID,
                              Quantity = detail.Quantity,
                              SubTotal = detail.SubTotal,

                              ProductID = product.ProductId,
                              ProductName = product.ProductName,
                              Category = product.Category,
                              Price = product.Price,
                              StockQuantity = product.StockQuantity,
                              SupplierID = product.SupplierID,
                              SupplierEmail = product.SupplierEmail,
                              ManufacturedDate = product.ManufacturedDate,
                              ExpiryDate = product.ExpiryDate,
                              Description = product.Description,
                              IsActive = product.IsActive,
                              ProductCreatedDate = product.CreatedDate,
                              ImageUrl = product.imageUrl
                          }).ToList();
            OrderVM? orderViewModel = result
                .GroupBy(x => new
                {
                    x.OrderID,
                    x.OrderDate,
                    x.CustomerName,
                    x.CustomerEmail,
                    x.TotalAmount,
                    x.OrderStatus,
                    x.OrderCreatedDate
                })
                .Select(group => new OrderVM
                {
                    OrderID = group.Key.OrderID,
                    OrderDate = group.Key.OrderDate,
                    CustomerName = group.Key.CustomerName,
                    CustomerEmail = group.Key.CustomerEmail,
                    TotalAmount = group.Key.TotalAmount,
                    OrderStatus = group.Key.OrderStatus,
                    CreatedDate = group.Key.OrderCreatedDate,
                    OrderDetails = group.Select(detail => new OrderDetailVM
                    {
                        OrderDetailID = detail.OrderDetailID,
                        Quantity = detail.Quantity,
                        SubTotal = detail.SubTotal,
                        ProductID = detail.ProductID
                    }).ToList()
                })
                .FirstOrDefault();
            if (orderViewModel == null)
            {
                return null;
            }
            return orderViewModel;

        }

        public async Task<List<IndexOrderVM>> GetAll()
        {
            try
            {
                var result = (await _unitOfWork.Orders.GetAllAsync()).Select(prod =>
                new IndexOrderVM()
                {
                    CustomerEmail = prod.CustomerEmail,
                    CustomerName = prod.CustomerName,
                    OrderDate = prod.OrderDate,
                    OrderID = prod.OrderID,
                    OrderStatus = prod.OrderStatus,
                    TotalAmount = prod.TotalAmount
                }).ToList();
                return result;

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error in OrderService.GetAll");
                return new List<IndexOrderVM>();
            }
        }

        public async Task Insert(OrderVM order)
        {
            var trackedProductIds = (await _unitOfWork.Products.GetAllAsync()).Select(x => x.ProductId);
            var untrackedProductIds = order.OrderDetails.Select(od=>od.ProductID).Except(trackedProductIds).ToList();

            if (untrackedProductIds.Count>0)
            {
                throw new Exception("invalid productIds detected.");
            }
            else
            {
                try
                {
                    Order orderEntity = new Order()
                    {
                        CreatedDate = DateTime.Now,
                        CustomerEmail = order.CustomerEmail,
                        CustomerName = order.CustomerName,
                        OrderDate = order.OrderDate,
                        OrderStatus = order.OrderStatus,
                        TotalAmount = order.TotalAmount
                    };
                    await _unitOfWork.Orders.AddAsync(orderEntity);
                    await _unitOfWork.CompleteAsync();
                    try
                    {
                        List<OrderDetail> orderDetailEntities = order.OrderDetails.Select(od => new OrderDetail()
                        {
                            OrderID=orderEntity.OrderID,
                            ProductID=od.ProductID,
                            Quantity=od.Quantity,
                            SubTotal=od.SubTotal
                        }).ToList();
                        await _unitOfWork.OrderDetails.BulkAddAsync(orderDetailEntities);
                        await _unitOfWork.CompleteAsync();
                    }
                    catch (Exception ex)
                    {
                        _unitOfWork.Orders.Delete(orderEntity);
                        await _unitOfWork.CompleteAsync();
                        throw;
                    }


                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "Error in IOrderService.Insert().");
                    throw;
                }
            }
        }

        public async Task Update(OrderVM order)
        {
            var trackedProductIds = (await _unitOfWork.Products.GetAllAsync()).Select(x => x.ProductId);
            var untrackedProductIds = order.OrderDetails.Select(od => od.ProductID).Except(trackedProductIds).ToList();

            if (untrackedProductIds.Count > 0)
            {
                throw new Exception("invalid productIds detected.");
            }
            else
            {
                try
                {
                    Order orderEntity = await _unitOfWork.Orders.GetByIdAsync(order.OrderID);
                    if (orderEntity == null)
                    {
                        throw new Exception("order not found");
                    }

                    orderEntity.CustomerEmail = order.CustomerEmail;
                    orderEntity.CustomerName = order.CustomerName;
                    orderEntity.OrderDate = order.OrderDate;
                    orderEntity.OrderStatus = order.OrderStatus;
                    orderEntity.TotalAmount = order.TotalAmount;
                    _unitOfWork.Orders.Update(orderEntity);

                    List<OrderDetail> dbOrderDetails = await _unitOfWork.OrderDetails.GetByFilter(od => od.OrderID == order.OrderID);
                    List<OrderDetail> orderDetailEntities = order.OrderDetails.Select(od => new OrderDetail()
                    {
                        OrderDetailID = od.OrderDetailID,
                        OrderID = od.OrderID == 0 ? orderEntity.OrderID : od.OrderID,
                        ProductID = od.ProductID,
                        Quantity = od.Quantity,
                        SubTotal = od.SubTotal
                    }).ToList();

                    var toDelete = dbOrderDetails
                        .Where(dbOD => !orderDetailEntities.Any(inOD => inOD.OrderDetailID == dbOD.OrderDetailID))
                        .ToList();

                    foreach (var detail in toDelete)
                    {
                        _unitOfWork.OrderDetails.Delete(detail);
                    }

                    foreach (var incoming in orderDetailEntities)
                    {
                        var existing = dbOrderDetails.FirstOrDefault(dbOD => dbOD.OrderDetailID == incoming.OrderDetailID);

                        if (existing != null)
                        {
                            existing.ProductID = incoming.ProductID;
                            existing.Quantity = incoming.Quantity;
                            existing.SubTotal = incoming.SubTotal;
                            _unitOfWork.OrderDetails.Update(existing);
                        }
                        else
                        {
                            await _unitOfWork.OrderDetails.AddAsync(incoming);
                        }
                    }

                    await _unitOfWork.CompleteAsync();


                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in IOrderService.Update().");
                    throw;
                }
            }

        }
    }
}
