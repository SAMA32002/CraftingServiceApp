using CraftingServiceApp.Application.Interfaces;
using CraftingServiceApp.Domain.Entities;

namespace CraftingServiceApp.BLL.Interfaces
{
    public class PostService : IPostService
    {
        private readonly IRepository<Post> _postRepository;

        public PostService(IRepository<Post> postRepository)
        {
            _postRepository = postRepository;
        }

        public IQueryable<Post> GetPostsByCategory(int categoryId)
        {
            return _postRepository.Find(s => s.CategoryId == categoryId);
        }

        public IQueryable<Post> GetPostsByClient(string clientId)
        {
            return _postRepository.Find(s => s.ClientId == clientId);
        }
    }
}
