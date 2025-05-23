namespace StoreManager.Features.EmailHelper
{
    public interface IEmailHelper
    {
        Task SendUnauthorizedEmailAsync();
    }
}
