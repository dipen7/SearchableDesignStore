namespace StoreManager.Features.EmailHelper.Model
{
    public class EmailConfig
    {
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUser { get; set; }
        public string SmtpPass { get; set; }
        public string AdminEmail { get; set; }
    }
}
