namespace CraftingServiceApp.Web.ViewModels
{
    public class SliderItemViewModel
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

}
