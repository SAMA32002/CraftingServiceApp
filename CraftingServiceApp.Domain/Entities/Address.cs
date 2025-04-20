
namespace CraftingServiceApp.Domain.Entities
{
    public class Address
    {
        public int Id { get; set; }
        public string ClientId { get; set; }
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public bool IsPrimary { get; set; } // Indicates default address
        public bool IsDeleted { get; set; } = false;

        // Navigation
        public ApplicationUser Client { get; set; }

        public string FullAddress()
        {
            return $"{Street}, {City}, {PostalCode}, {Country}";
        }
    }

}
