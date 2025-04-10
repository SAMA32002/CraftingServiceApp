using CraftingServiceApp.Domain.Entities;

namespace CraftingServiceApp.Web.ViewModels
{
    public class HomePageViewModel
    {
        public List<Service> RecentServices { get; set; }
        public List<SliderItemViewModel> ActiveSliders { get; set; }
    }

}
