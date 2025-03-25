namespace CraftingServiceApp.AdminAPI.Dtos
{
        public class RequestDTO
        {
            public int Id { get; set; }
            public string ClientId { get; set; }  // Client's ID
            public int ServiceId { get; set; }    // Service's ID
            public DateTime RequestDate { get; set; }
            public string Status { get; set; }  // You may want to use the enum if possible
            public int? SelectedScheduleId { get; set; }  // SelectedSchedule's ID (nullable)
            public DateTime? ScheduledDateTime { get; set; }
            public string Notes { get; set; }
            public string PaymentStatus { get; set; }  // You may want to use the enum if possible
            public List<int> ProposedDates { get; set; }  // List of Proposed Schedule IDs
            public int? SelectedAddressId { get; set; }  // SelectedAddress ID (nullable)
            public string PaymentIntentId { get; set; }
        }
    }


