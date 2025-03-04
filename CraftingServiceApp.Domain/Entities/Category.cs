
namespace CraftingServiceApp.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        // Relationship: One Category can have many Services
        public virtual ICollection<Service> Services { get; set; }
    }

}
