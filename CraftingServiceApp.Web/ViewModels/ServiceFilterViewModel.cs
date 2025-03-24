namespace CraftingServiceApp.Web.ViewModels
{
    public class ServiceFilterViewModel
    {
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SortBy { get; set; } // Options: "price_asc", "price_desc"
    }
}
