namespace CraftingServiceApp.AdminAPI.Dtos
{
    public class SliderItemDTO
    {
        public IFormFile MediaFile { get; set; }  // File to upload (image/video)
        public string? Description { get; set; }  // Optional description for the slider item
        public string FileType { get; set; }  // "Image" or "Video" (to distinguish between the media types)
        public bool IsActive { get; set; } = true;
    }

}
