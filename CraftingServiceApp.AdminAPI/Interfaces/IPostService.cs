using CraftingServiceApp.AdminAPI.Dtos;
using CraftingServiceApp.AdminAPI.Helpers;

namespace CraftingServiceApp.AdminAPI.Interfaces
{
    public interface IPostService
    {
        Task<PagedResult<PostDto>> GetAllPostsAsync(PaginationParameters paginationParameters, QueryOptions queryOptions);
        Task<PostDto> GetPostByIdAsync(int id);
        Task<bool> DeletePostAsync(int id);
        Task<int> GetPostCountAsync();
    }
}
