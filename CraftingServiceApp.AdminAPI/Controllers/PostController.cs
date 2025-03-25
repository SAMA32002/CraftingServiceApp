using CraftingServiceApp.AdminAPI.Helpers;
using CraftingServiceApp.AdminAPI.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CraftingServiceApp.AdminAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly ILogger<PostController> _logger;

        public PostController(IPostService postService, ILogger<PostController> logger)
        {
            _postService = postService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPosts([FromQuery] PaginationParameters paginationParameters, [FromQuery] QueryOptions queryOptions)
        {
            try
            {
                var posts = await _postService.GetAllPostsAsync(paginationParameters, queryOptions);
                return Ok(new { Message = "Posts retrieved successfully.", Data = posts });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve posts.");
                return StatusCode(500, new { Message = "An error occurred while retrieving posts." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostById(int id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null) return NotFound(new { Message = "Post not found." });
            return Ok(new { Message = "Post retrieved successfully.", Data = post });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var result = await _postService.DeletePostAsync(id);
            if (!result) return NotFound(new { Message = "Post not found." });
            return Ok(new { Message = "Post deleted successfully." });
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetPostCount()
        {
            var count = await _postService.GetPostCountAsync();
            return Ok(new { Message = "Post count retrieved successfully.", Count = count });
        }
    }
}
