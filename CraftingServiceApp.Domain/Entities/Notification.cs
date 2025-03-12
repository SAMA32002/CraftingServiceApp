
using System.ComponentModel.DataAnnotations;

namespace CraftingServiceApp.Domain.Entities
{
    public class Notification
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; } // The recipient (Crafter/Client)

        [Required]
        public string Message { get; set; }

        public bool IsRead { get; set; } = false; // Mark unread notifications

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
