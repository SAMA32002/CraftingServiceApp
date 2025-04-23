
using CraftingServiceApp.Domain.Enums;

namespace CraftingServiceApp.Domain.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public string StripePaymentIntentId { get; set; }
        public string StripeClientSecret { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReleasedAt { get; set; }
        public DateTime? DisputedAt { get; set; }

        // Foreign keys
        public int RequestId { get; set; }
        public string ClientId { get; set; }
        public string CrafterId { get; set; }

        // Navigation properties
        public Request Request { get; set; }
        public ApplicationUser Client { get; set; }
        public ApplicationUser Crafter { get; set; }
    }    
}
