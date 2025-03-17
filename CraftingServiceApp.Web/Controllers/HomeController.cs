using CraftingServiceApp.Application.Interfaces;
using CraftingServiceApp.Infrastructure.Data;
using CraftingServiceApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CraftingServiceApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserRepository _userRepository;

        public HomeController(ApplicationDbContext context, IUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }

        // عرض الصفحة الرئيسية مع آخر الخدمات
        public async Task<IActionResult> Index()
        {
            // بنجيب أحدث 4 خدمات
            var recentServices = await _context.Services
                .Include(s => s.Category)
                .Include(s => s.Crafter)
                .OrderByDescending(s => s.Id)
                .Take(4) // ممكن تغيري العدد اللي يعجبك
                .ToListAsync();

            // بنرجع البيانات للـ View
            return View(recentServices);
        }

        // EndPoint لو حبيتي تجيبي الخدمات بعدد معين (API/Partial View)
        public async Task<IActionResult> RecentService(int count = 4)
        {
            if (count <= 0)
            {
                return BadRequest("Count must be greater than zero.");
            }

            var services = await _context.Services
                .Include(s => s.Category)
                .Include(s => s.Crafter)
                .OrderByDescending(s => s.Id)
                .Take(count)
                .ToListAsync();

            // ممكن ترجعي View أو Json حسب ما تحبي
            return PartialView("_RecentServicesPartial", services);
        }

        // Error page
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}