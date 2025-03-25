namespace CraftingServiceApp.AdminAPI.Dtos
{
    public class PostDto
    {
        public int Id { get; set; }
        public string ClientId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
    }
}
