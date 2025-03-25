namespace CraftingServiceApp.AdminAPI.Interfaces
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file, string fileType);
        Task<bool> DeleteFileAsync(string filePath);
    }
}
