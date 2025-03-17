using CraftingServiceApp.Domain.Entities;

namespace CraftingServiceApp.Web.ViewModels
{
    public class ProfileViewModel
    {
        public ApplicationUser User { get; set; }
        public bool IsCrafter { get; set; }
        public IFormFile ProfileImage { get; set; }
        public List<Service> Services { get; set; }
        public List<Request> ReceivedRequests { get; set; } // For crafters
        public List<Request> SentRequests { get; set; } // For clients
        public List<Post> Posts { get; set; } // For clients
        public List<Address> Addresses { get; set; }
    }
}
