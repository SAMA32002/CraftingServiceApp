
namespace CraftingServiceApp.Common.DTOs
{
    public class UserDTO
    {
        public string Id { get; set; }  // ✅ Ensure ID is string (IdentityUser uses string IDs)
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int Role { get; set; }  // ✅ Use int instead of Enum for DTOs
        public bool IsBanned { get; set; }
    }
}
