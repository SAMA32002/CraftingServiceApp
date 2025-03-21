
using CraftingServiceApp.Domain.Enums;

namespace CraftingServiceApp.Domain.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public string ClientId { get; set; } // The paying client
        public string CrafterId { get; set; } // The crafter receiving the payment
        public int RequestId { get; set; }
        public Request Request { get; set; }

        public int ServiceId { get; set; }  // The purchased service
        public decimal Amount { get; set; } // Payment amount
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // 🔗 Stripe Payment Intent Details
        public string? PaymentIntentId { get; set; } // Stores Stripe's Payment ID
        public string? ClientSecret { get; set; } // Stores Stripe’s ClientSecret

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public bool IsSuccess { get; set; } = false; // Tracks if payment was successful
    }

}
