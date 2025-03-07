using System.ComponentModel.DataAnnotations;

namespace CraftingServiceApp.Domain.Entities
{
    public class Request
    {
        public int Id { get; set; }

        [Required]
        public string ClientId { get; set; }
        public ApplicationUser Client { get; set; }

        [Required]
        public int ServiceId { get; set; }
        public Service Service { get; set; }

        [Required]
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(50)]
        public RequestStatus Status { get; set; } = RequestStatus.Pending;

        [StringLength(500)]
        public string Notes { get; set; }

        public int? PaymentId { get; set; }
        public Payment Payment { get; set; }
    }

    public enum RequestStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2,
        Completed = 3
    }

}
