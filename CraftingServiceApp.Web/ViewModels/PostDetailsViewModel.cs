using CraftingServiceApp.Domain.Entities;

namespace CraftingServiceApp.Web.ViewModels
{
    public class PostDetailsViewModel
    {
        public Post Post { get; set; }
        public string ClientId { get; set; }
        public List<CommentViewModel> Comments { get; set; }
    }
}
