
namespace CraftingServiceApp.Domain.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string ClientId { get; set; } // Who created the post
        public string Title { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }  // Foreign Key
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public PostStatus Status { get; set; } = PostStatus.Open; // Enum
        public virtual ApplicationUser? Client { get; set; } // Who posted
        public virtual Category? Category { get; set; }  // Navigation Property
        public virtual ICollection<Comment>? Comments { get; set; } // Crafter comments
    }

    public enum PostStatus
    {
        Open,       // Still accepting crafters
        InProgress, // Service started
        Closed      // Service completed
    }


}
