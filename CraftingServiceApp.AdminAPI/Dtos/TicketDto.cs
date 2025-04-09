using CraftingServiceApp.Domain.Entities;

namespace CraftingServiceApp.AdminAPI.Dtos
{
    public class TicketDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public TicketStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateTicketDto
    {
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }

    public class UpdateTicketDto
    {
        public TicketStatus Status { get; set; }
    }
}
