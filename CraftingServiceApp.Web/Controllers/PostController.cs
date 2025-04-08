using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CraftingServiceApp.Domain.Entities;
using CraftingServiceApp.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using CraftingServiceApp.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using CraftingServiceApp.Web.ViewModels;

namespace CraftingServiceApp.Web.Controllers
{
    public class PostController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Post> _PostRepository;
        private readonly IRepository<Category> _CategoRyrepository;
        private readonly IRepository<Comment> _CommentRepository;
        private readonly IPostService _PostsService;

        public PostController(UserManager<ApplicationUser> userManager, IRepository<Post> postRepository, IRepository<Category> categoRyrepository, IRepository<Comment> commentRepository, IPostService postsService)
        {
            _userManager = userManager;
            _PostRepository = postRepository;
            _CategoRyrepository = categoRyrepository;
            _CommentRepository = commentRepository;
            _PostsService = postsService;
        }

        // GET: Post
        public async Task<IActionResult> Index(int? categoryId)
        {
            if (User.IsInRole("Client"))
            {
                var userId = _userManager.GetUserId(User);
                var user = await _userManager.FindByIdAsync(userId);
                ViewBag.IsBanned = user.IsBanned;
            }
            var posts = await _PostRepository.GetAllAsync();

            // إرسال كل الكاتيجوريز للـ ViewData
            ViewData["Categories"] = new SelectList(_CategoRyrepository.GetAll(), "Id", "Name");

            if (categoryId.HasValue)
            {
                posts = _PostsService.GetPostsByCategory(categoryId.Value);
            }

            return View(posts);
        }

        // GET: Post/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var post = await _PostRepository.GetAll()
                .Include(p => p.Category)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.Crafter)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null) return NotFound();

            // تحويل البيانات إلى ViewModel
            var postComments = post.Comments.Select(comment => new CommentViewModel
            {
                Message = comment.Message,
                CrafterId = comment.CrafterId,
                CrafterName = comment.Crafter?.UserName
            }).ToList();

            var viewModel = new PostDetailsViewModel
            {
                Post = post,
                ClientId = post.ClientId,
                Comments = postComments
            };

            ViewData["Categories"] = new SelectList(_CategoRyrepository.GetAll(), "Id", "Name");

            return View(viewModel);
        }


        // GET: Post/Create
        public IActionResult Create()
        {
            // إرسال كل الكاتيجوريز للـ ViewData
            ViewData["Categories"] = new SelectList(_CategoRyrepository.GetAll(), "Id", "Name");

            return View();
        }

        // POST: Post/Create
        [Authorize(Roles = "Client")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.IsBanned == true)
            {
                return Json(new { success = false, message = "You are banned from posting." });
            }

            post.ClientId = _userManager.GetUserId(User);

            ModelState.Clear();
            TryValidateModel(post);

            if (ModelState.IsValid)
            {
                _PostRepository.Add(post);
                await _PostRepository.SaveAsync();
                return Json(new { success = true, redirectUrl = Url.Action(nameof(Index)) });
            }

            // If we got this far, something failed
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return Json(new { success = false, message = "Validation failed", errors });
        }

        // GET: Post/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _PostRepository.GetByIdAsync(id);
            if (post == null) return NotFound();

            // إرسال كل الكاتيجوريز للـ ViewData مع تحديد العنصر المختار
            ViewData["Categories"] = new SelectList(_CategoRyrepository.GetAll(), "Id", "Name", post.CategoryId);

            return View(post);
        }

        // POST: Post/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Post post)
        {
            if (id != post.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _PostRepository.Update(post);
                    await _PostRepository.SaveAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    // تقدر تضيف لوج هنا لو حبيت
                    throw;
                }
            }

            // إعادة تحميل الكاتيجوريز في حالة فشل التحقق من صحة الموديل
            ViewData["Categories"] = new SelectList(_CategoRyrepository.GetAll(), "Id", "Name", post.CategoryId);

            return View(post);
        }

        // GET: Post/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _PostRepository.GetByIdAsync(id);

            if (post == null) return NotFound();

            return View(post);
        }

        // POST: Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _PostRepository.GetByIdAsync(id);
            if (post != null)
            {
                _PostRepository.Delete(post);
                await _PostRepository.SaveAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Client")]
        public async Task<IActionResult> CheckBanStatus()
        {
            var user = await _userManager.GetUserAsync(User);
            return Json(new { isBanned = user?.IsBanned ?? false });
        }
    }
}
