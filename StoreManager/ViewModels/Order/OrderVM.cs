
namespace StoreManager.ViewModels.Order
{
    public class OrderVM
    {
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public decimal TotalAmount { get; set; }
        public bool OrderStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<OrderDetailVM> OrderDetails { get; set; }
        

    }
}
