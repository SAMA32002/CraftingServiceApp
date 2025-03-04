
namespace CraftingServiceApp.Domain.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string CrafterId { get; set; }  // Who commented
        public int PostId { get; set; }        // Related post
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ApplicationUser Crafter { get; set; }
        public Post Post { get; set; }
    }

}
