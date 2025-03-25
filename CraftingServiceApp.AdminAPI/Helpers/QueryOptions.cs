namespace CraftingServiceApp.AdminAPI.Helpers
{
    public class QueryOptions
    {
        public string? Search { get; set; }
        public string SortField { get; set; } = "Date"; 
        public bool SortDescending { get; set; } = false;
        public string? Name { get; set; }
        public DateTime? StartDate { get; set; } 
        public DateTime? EndDate { get; set; }   
    }
}
