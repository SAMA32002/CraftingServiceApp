using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace CraftingServiceApp.Domain.Entities
{
    public class Service
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string? Image { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public int CategoryId { get; set; }
        public string CrafterId { get; set; }
        public Category? Category { get; set; }
        public ApplicationUser? Crafter { get; set; }
        public virtual ICollection<Review>? Reviews { get; set; }
        public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
    }

}
