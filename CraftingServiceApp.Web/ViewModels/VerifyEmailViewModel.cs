using System.ComponentModel.DataAnnotations;

namespace CraftingServiceApp.Web.ViewModels
{
    public class VerifyEmailViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; }
    }

}
