using CraftingServiceApp.Domain.Entities;

namespace CraftingServiceApp.Web.ViewModels
{
    public class ServiceDetailsViewModel
    {
        public Service Service { get; set; }
        public double AverageRating { get; set; }
        public Review Review { get; set; } // For new review submission
    }
}
