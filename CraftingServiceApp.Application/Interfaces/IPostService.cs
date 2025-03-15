using CraftingServiceApp.Domain.Entities;

namespace CraftingServiceApp.BLL.Interfaces
{
    public interface IPostService
    {
        IQueryable<Post> GetPostsByCategory(int categoryId);
        IQueryable<Post> GetPostsByClient(string clientId);
    }
}
