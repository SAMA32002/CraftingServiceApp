using System.ComponentModel.DataAnnotations;

namespace CraftingServiceApp.Web.ViewModels
{
    public class TicketViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string Subject { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 10)]
        public string Message { get; set; }
    }

}
