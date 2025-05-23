namespace StoreManager.ViewModels.Order
{
    public class IndexOrderVM
    {
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public decimal TotalAmount { get; set; }
        public bool OrderStatus { get; set; }
    }
}
