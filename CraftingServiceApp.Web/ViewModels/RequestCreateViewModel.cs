using CraftingServiceApp.Domain.Entities;

namespace CraftingServiceApp.Web.ViewModels
{
    public class RequestCreateViewModel
    {
        public int ServiceId { get; set; }
        public string ServiceTitle { get; set; } // To display service title

        public DateTime ProposedDate1 { get; set; }
        public DateTime? ProposedDate2 { get; set; }
        public DateTime? ProposedDate3 { get; set; }
        public string? Notes { get; set; }

        // Address Selection
        public bool UseExistingAddress { get; set; } = true; // Default to using saved address
        public int? SelectedAddressId { get; set; } // Selected saved address
        public List<Address> ClientAddresses { get; set; } = new(); // List of saved addresses

        // If client enters a new address
        public string? NewStreet { get; set; }
        public string? NewCity { get; set; }
        public string? NewPostalCode { get; set; }
        public string? NewCountry { get; set; }
    }

}
