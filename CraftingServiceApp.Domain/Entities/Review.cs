
namespace CraftingServiceApp.Domain.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public string ClientId { get; set; }
        public int ServiceId { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        public ApplicationUser? Client { get; set; }
        public Service? Service { get; set; }
    }

}
