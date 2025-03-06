
namespace CraftingServiceApp.Domain.Enums
{
    public enum PaymentStatus
    {
        Pending,   // Payment not completed
        Completed, // Payment successful
        Failed,    // Payment failed
        Refunded   // Payment refunded
    }
}
