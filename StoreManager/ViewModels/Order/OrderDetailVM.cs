namespace StoreManager.ViewModels.Order
{
    public class OrderDetailVM
    {
        public int OrderDetailID { get; set; }
        public int OrderID { get; set; }//not needed
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }
    }
}
