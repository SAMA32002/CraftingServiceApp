
using CraftingServiceApp.Domain.Enums;

namespace CraftingServiceApp.Domain.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public string ClientId { get; set; }  // Who paid
        public string CrafterId { get; set; } // Who received payment
        public int ServiceId { get; set; }    // Paid Service
        public decimal Amount { get; set; }   // Payment Amount
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending; // Enum
        public ApplicationUser Client { get; set; }
        public ApplicationUser Crafter { get; set; }
        public Service Service { get; set; }
    }
}
