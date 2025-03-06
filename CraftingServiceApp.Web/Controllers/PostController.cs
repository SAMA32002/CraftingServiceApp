using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CraftingServiceApp.Domain.Entities;
using CraftingServiceApp.Infrastructure.Data;
using CraftingServiceApp.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using CraftingServiceApp.Web.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using CraftingServiceApp.BLL.Interfaces;
using Stripe;

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
            var posts = await _PostRepository.GetAllAsync();
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
            var post = await _PostRepository.GetByIdAsync(id);
            ViewData["CategoryId"] = new SelectList(_CategoRyrepository.GetAll(), "Id", "Name");

            if (post == null) return NotFound();

            return View(post);
        }

        // GET: Post/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_CategoRyrepository.GetAll(), "Id", "Name"); 
            return View();
        }

        // POST: Post/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClientId,Title,Description,CategoryId,Status")] Post post)
        {
            if (ModelState.IsValid)
            {
                _PostRepository.Add(post);
                await _PostRepository.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Post/Edit/5
        public async Task<IActionResult> Edit(int id)
        {

            var post = await _PostRepository.GetByIdAsync(id);
            if (post == null) return NotFound();

            return View(post);
        }

        // POST: Post/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClientId,Title,Description,CategoryId,Status")] Post post)
        {
            if (id != post.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _PostRepository.Update(post);
                    await _PostRepository.SaveAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Post/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null) return NotFound();

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
    }
}