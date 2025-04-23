using System.ComponentModel.DataAnnotations;

namespace CraftingServiceApp.Web.ViewModels
{
    public class PaymentViewModel
    {
        public int PaymentId { get; set; }
        public int RequestId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        public string ServiceName { get; set; }
    }

    public class ProcessPaymentViewModel
    {
        public int PaymentId { get; set; }
        public string ClientSecret { get; set; }
        public decimal Amount { get; set; }
        public int RequestId { get; set; }
    }
}
