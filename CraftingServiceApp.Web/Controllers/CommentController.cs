using CraftingServiceApp.Application.Interfaces;
using CraftingServiceApp.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CraftingServiceApp.Web.Controllers
{
    [Authorize(Roles = "Crafter")] // فقط الصنايعي يكتب كومنت
    public class CommentController : Controller
    {
        private readonly IRepository<Comment> _commentRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentController(IRepository<Comment> commentRepo, UserManager<ApplicationUser> userManager)
        {
            _commentRepo = commentRepo;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int postId, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                TempData["Error"] = "Comment cannot be empty.";
                return RedirectToAction("Details", "Post", new { id = postId });
            }

            var userId = _userManager.GetUserId(User);
            var comment = new Comment
            {
                PostId = postId,
                CrafterId = userId,
                Message = message,
                CreatedAt = DateTime.UtcNow
            };

            _commentRepo.Add(comment);
            await _commentRepo.SaveAsync();

            // إضافة رسالة نجاح
            TempData["SuccessMessage"] = "Comment added successfully.";

            return RedirectToAction("Details", "Post", new { id = postId });
        }
    }
}
