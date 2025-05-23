namespace StoreManager.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string DisplayName { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsMember { get; set; }
    }
}
