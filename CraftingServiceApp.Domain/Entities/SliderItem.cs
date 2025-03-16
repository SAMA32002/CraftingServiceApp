namespace CraftingServiceApp.Domain.Entities
{
    public class SliderItem
    {
        public int Id { get; set; }
        public string FilePath { get; set; } = string.Empty; // Store the file name
        public string FileType { get; set; } = string.Empty; // "Image" or "Video"
        public bool IsActive { get; set; } // To control visibility
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
