using System.ComponentModel.DataAnnotations;

namespace StoreManager.ViewModels.Product
{
    public class ProductWithImage
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string SupplierEmail { get; set; }
        public DateTime ManufacturedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public IFormFile ProductImage { get; set; }
        public string ImageUrl { get; set; }
    }
}
