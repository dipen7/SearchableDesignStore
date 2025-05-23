namespace StoreManager.Features.ImageHelper
{
    public interface IImageHelper
    {
        Task<string> UploadFIle(IFormFile file, string folderPath, string folderName);
        void DeleteFile(string filepath);
    }
}
