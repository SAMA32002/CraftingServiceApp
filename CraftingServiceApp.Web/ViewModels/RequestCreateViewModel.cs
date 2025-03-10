using System.ComponentModel.DataAnnotations;

namespace CraftingServiceApp.Web.ViewModels
{
    public class RequestCreateViewModel
    {
        public int ServiceId { get; set; }
        public string ServiceTitle { get; set; } // To display service title
        public DateTime ProposedDate1 { get; set; }
        public DateTime? ProposedDate2 { get; set; }
        public DateTime? ProposedDate3 { get; set; }
        public string Notes { get; set; }
    }
}
