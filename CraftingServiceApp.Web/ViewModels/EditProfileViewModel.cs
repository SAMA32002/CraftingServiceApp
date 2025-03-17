using System.ComponentModel.DataAnnotations;

namespace CraftingServiceApp.Web.ViewModels
{
    public class EditProfileViewModel
    {
        public string Id { get; set; }
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string? NewPassword { get; set; }
        [Display(Name = "Profile Picture")]
        public IFormFile? ProfilePicture { get; set; }
        public string? ExistingProfilePicture { get; set; }
        public List<AddressViewModel> Addresses { get; set; } = new List<AddressViewModel>();
    }
}
