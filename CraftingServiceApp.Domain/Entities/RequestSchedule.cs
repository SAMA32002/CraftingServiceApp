using System.ComponentModel.DataAnnotations;

namespace CraftingServiceApp.Domain.Entities
{
    public class RequestSchedule
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public Request Request { get; set; }

        [Required]
        public DateTime ProposedDate { get; set; }

        public bool IsSelected { get; set; } = false; // Crafter marks one as true
    }

}
