using CraftingServiceApp.Domain.Enums;
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
        public RequestStatus Status { get; set; } = RequestStatus.Pending;

        // 🔗 Stores which proposed schedule was selected
        public int? SelectedScheduleId { get; set; }
        public RequestSchedule SelectedSchedule { get; set; }

        // 📅 Stores the final confirmed appointment date
        public DateTime? ScheduledDateTime { get; set; }
        public List<RequestSchedule> ProposedDates { get; set; } = new();

        [StringLength(500)]
        public string? Notes { get; set; }

        public int? PaymentId { get; set; }
        public Payment Payment { get; set; }
        public decimal TotalAmount { get; set; } // Added to store the payment amount

        // Status flags for payment workflow
        public bool IsApproved { get; set; } = false; // Added for crafter approval
        public bool IsCompleted { get; set; } = false; // Added for completion status
        public bool IsDisputed { get; set; } = false; // Added for dispute tracking

        // ✅ New Fields for Address Selection
        public int? SelectedAddressId { get; set; } // If they use a saved address
        public Address SelectedAddress { get; set; } // Navigation property

        public string? CustomStreet { get; set; } // If they enter a new address
        public string? CustomCity { get; set; }
        public string? CustomPostalCode { get; set; }
        public string? CustomCountry { get; set; }
        // Stripe payment identifiers
        public string? PaymentIntentId { get; set; } // Kept for backward compatibility
        public string? StripeClientSecret { get; set; } // Added for client-side payment processing
    }
}
