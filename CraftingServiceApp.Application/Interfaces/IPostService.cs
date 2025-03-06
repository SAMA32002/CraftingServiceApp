using CraftingServiceApp.Domain.Entities;

namespace CraftingServiceApp.BLL.Interfaces
{
    public interface IPostService
    {
        IEnumerable<Post> GetPostsByCategory(int categoryId);
        IEnumerable<Post> GetPostsByClient(string clientId);
    }
}
