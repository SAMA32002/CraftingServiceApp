using CraftingServiceApp.Domain.Enums;

namespace CraftingServiceApp.Web.ViewModels
{
    public class RequestDetailsViewModel
    {
        public string ServiceTitle { get; set; }
        public RequestStatus Status { get; set; }
        public DateTime RequestDate { get; set; }
        public List<DateTime> ProposedDates { get; set; } = new();
        public string Notes { get; set; }
    }
}
