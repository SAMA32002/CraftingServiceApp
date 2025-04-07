using System.ComponentModel.DataAnnotations;

namespace CraftingServiceApp.Web.ViewModels
{
    public class UserRegistrationViewModel
    {
        //public string Id { get; set; }
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Profile Picture")]
        public IFormFile ProfilePicture { get; set; }

        public bool IsBanned { get; set; } = false;

        [Required]
        [Display(Name = "Role")]
        public string RoleId { get; set; }
        public List<AddressViewModel> Addresses { get; set; } = new List<AddressViewModel>();
    }

    public class AddressViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Street is required")]
        [StringLength(100, ErrorMessage = "Street cannot exceed 100 characters")]
        public string Street { get; set; }

        [Required(ErrorMessage = "City is required")]
        [StringLength(50, ErrorMessage = "City cannot exceed 50 characters")]
        public string City { get; set; }

        [Required(ErrorMessage = "Postal Code is required")]
        [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Invalid Postal Code format")]
        public string PostalCode { get; set; }
        //public string Country { get; set; }
        public bool IsPrimary { get; set; }

        public string FullAddress()
        {
            return $"{Street}, {City}, {PostalCode}";
        }
    }
}
