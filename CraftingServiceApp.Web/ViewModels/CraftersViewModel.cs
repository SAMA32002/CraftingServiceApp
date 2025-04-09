using System.ComponentModel.DataAnnotations;

namespace CraftingServiceApp.Web.ViewModels
{
    //public class CraftersViewModel
    //{
    //    [Required]
    //    [Display(Name = "Full Name")]
    //    public string FullName { get; set; }

    //    [Required]
    //    [Phone]
    //    [Display(Name = "Phone Number")]
    //    public string PhoneNumber { get; set; }

    //    [Display(Name = "Profile Picture")]
    //    public string ProfilePic { get; set; }

    //}
    public class CraftersViewModel
    {
        public string Id { get; set; } // << أضفنا الـ Id هنا

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Profile Picture")]
        public string ProfilePic { get; set; }
    }

}
