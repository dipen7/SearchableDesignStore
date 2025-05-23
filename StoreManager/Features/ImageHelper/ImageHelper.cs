
using Microsoft.AspNetCore.Hosting;

namespace StoreManager.Features.ImageHelper
{
    public class ImageHelper : IImageHelper
    {
        public async Task<string> UploadFIle(IFormFile file, string folderPath, string folderName)
        {
            if (file == null || file.Length == 0)
            {
                throw new Exception("File is null or empty");
            }

            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(folderPath, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return folderName + "/" + uniqueFileName;


            //var fileName = Path.GetFileName(file.FileName);
            //var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);
            //using (var stream = new FileStream(filePath, FileMode.Create))
            //{
            //    file.CopyTo(stream);
            //}
            //return $"/images/{fileName}";
        }
        public void DeleteFile(string filepath)
        {
                if (System.IO.File.Exists(filepath))
                {
                    System.IO.File.Delete(filepath);
                }
        }
    }
}
