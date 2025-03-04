using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CraftingServiceApp.Domain.Entities;
using System.Linq;
using System.Threading.Tasks;
using CraftingServiceApp.Infrastructure.Data;

namespace CraftingServiceApp.Web.Controllers
{
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CommentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Comment
        public async Task<IActionResult> Index()
        {
            var comments = await _context.Comments.Include(c => c.Crafter).Include(c => c.Post).ToListAsync();
            return View(comments);
        }

        // GET: Comment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var comment = await _context.Comments.Include(c => c.Crafter).Include(c => c.Post)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (comment == null) return NotFound();

            return View(comment);
        }

        // GET: Comment/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Comment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CrafterId,PostId,Message")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(comment);
        }

        // GET: Comment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null) return NotFound();

            return View(comment);
        }

        // POST: Comment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CrafterId,PostId,Message")] Comment comment)
        {
            if (id != comment.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Comments.Any(e => e.Id == comment.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(comment);
        }
    }
}
