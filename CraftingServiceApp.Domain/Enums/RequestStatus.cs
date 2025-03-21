
namespace CraftingServiceApp.Domain.Enums
{
    public enum RequestStatus
    {
        Pending = 0,    // Waiting for crafter to accept
        Accepted = 1,   // Crafter accepted
        Rejected = 2,   // Crafter rejected
        Scheduled = 3,  // Appointment confirmed
        Paid = 4,       // Payment completed
        Completed = 5,  // Service finished
        Expired = 6,    // Crafter didn't confirm in time
        Refunded = 7,   // Client requests a refund
    }
}
