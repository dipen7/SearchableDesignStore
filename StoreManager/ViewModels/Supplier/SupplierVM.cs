using System.ComponentModel.DataAnnotations;

namespace StoreManager.ViewModels.Supplier
{
    public class SupplierVM
    {
        public int SupplierID { get; set; }
        [MaxLength(100)]
        [Required(ErrorMessage = "Supplier Name is required.")]
        public string SupplierName { get; set; }
        [MaxLength(20)]
        [Required(ErrorMessage ="Contact Number is required.")]
        [DataType(DataType.PhoneNumber)]
        public string ContactNumber { get; set; }
        [Required(ErrorMessage ="Email is required.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
