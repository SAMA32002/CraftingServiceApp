using CraftingServiceApp.AdminAPI.Interfaces;

namespace CraftingServiceApp.AdminAPI.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env; // For local file storage (you can use cloud storage in real apps)

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string fileType)
        {
            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            if (!Directory.Exists(uploads))
            {
                Directory.CreateDirectory(uploads);
            }

            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(uploads, fileName);

            // Depending on the file type (Image or Video), you can add specific handling here if needed
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/{fileName}";
        }

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            var fullPath = Path.Combine(_env.WebRootPath, filePath.TrimStart('/'));

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return true;
            }

            return false;
        }
    }
}