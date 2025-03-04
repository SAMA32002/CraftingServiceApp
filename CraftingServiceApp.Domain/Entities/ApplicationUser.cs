using CraftingServiceApp.Domain.Enums;
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
        public UserRole Role { get; set; } // Crafter, Client, Admin
        public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
        public virtual ICollection<Service> Services { get; set; }
    }
}
