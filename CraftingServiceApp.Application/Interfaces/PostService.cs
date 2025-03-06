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

        public IEnumerable<Post> GetPostsByCategory(int categoryId)
        {
            return _postRepository.Find(s => s.CategoryId == categoryId);
        }

        public IEnumerable<Post> GetPostsByClient(string clientId)
        {
            return _postRepository.Find(s => s.ClientId == clientId);
        }
    }
}
