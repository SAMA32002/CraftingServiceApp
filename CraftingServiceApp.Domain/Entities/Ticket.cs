
namespace CraftingServiceApp.Domain.Entities
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Email { get; set; } // Anonymous users can submit tickets
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public TicketStatus Status { get; set; } = TicketStatus.Open; // Enum
    }

    public enum TicketStatus
    {
        Open,        // Waiting for a response
        InProgress,  // Ticket being handled
        Resolved,    // Issue resolved
        Closed       // No further action needed
    }

}

