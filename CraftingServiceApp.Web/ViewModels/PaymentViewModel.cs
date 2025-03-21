using CraftingServiceApp.Domain.Enums;

namespace CraftingServiceApp.Web.ViewModels
{
    public class PaymentViewModel
    {
        public int RequestId { get; set; } 
        public string ClientId { get; set; } 
        public string CrafterId { get; set; } 
        public string PaymentIntentId { get; set; } 
        public string ClientSecret { get; set; } 
        public decimal Amount { get; set; } 
        public string Currency { get; set; } = "EGP"; 
        public PaymentStatus PaymentStatus { get; set; }
        public int ServiceId { get; set; }
        public string ServiceTitle { get; set; }
        public string PublishableKey { get; set; }
    }

}
