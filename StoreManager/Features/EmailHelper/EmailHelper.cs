using StoreManager.Features.EmailHelper.Model;
using System.Net;
using System.Net.Mail;

namespace StoreManager.Features.EmailHelper
{
    public class EmailHelper:IEmailHelper
    {
        private readonly ILogger<IEmailHelper> _logger;
        private readonly EmailConfig _emailConfig;

        public EmailHelper(ILogger<IEmailHelper> logger, IConfiguration configuration)
        {
            _logger = logger;
            _emailConfig = configuration.GetSection("EmailConfig").Get<EmailConfig>();
        }

        public async Task SendUnauthorizedEmailAsync()
        {
            try
            {
                string templatePath = Path.Combine(AppContext.BaseDirectory, "Templates", "unauthorized_template.html");
                string template = await File.ReadAllTextAsync(templatePath);
                var message = new MailMessage
                {
                    From = new MailAddress(_emailConfig.SmtpUser),
                    Subject = "Unauthorized access try.",
                    Body = template,
                    IsBodyHtml = true
                };

                message.To.Add(_emailConfig.AdminEmail);

                using (var client = new SmtpClient(_emailConfig.SmtpHost, _emailConfig.SmtpPort))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(_emailConfig.SmtpUser, _emailConfig.SmtpPass);
                    await client.SendMailAsync(message);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            
        }
    }
}
