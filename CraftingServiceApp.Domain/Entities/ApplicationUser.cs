using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CraftingServiceApp.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        [Required(ErrorMessage = "Phone number is required")]
        [Phone]
        public string PhoneNumber { get; set; }

        public string? ProfilePic { get; set; } // Optional

        //public string Title { get; set; }
        public bool IsBanned { get; set; } = false;
        public string RoleId { get; set; }
        public IdentityRole Role { get; set; }
        //public UserRole Role { get; set; } // Crafter, Client, Admin
        public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
        public virtual ICollection<Service> Services { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public List<Request> SentRequests { get; set; } = new();  // Requests made by the client
        public List<Request> ReceivedRequests { get; set; } = new(); // Requests received as a crafter
    }
}
