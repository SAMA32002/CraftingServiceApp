using CraftingServiceApp.Infrastructure.Data;
using CraftingServiceApp.Web.Models;
using CraftingServiceApp.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // مهم لو بتستخدم Include
using System.Diagnostics;


namespace CraftingServiceApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context; // أضفنا الـ DbContext هنا

        // Inject both ILogger و DbContext
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Crafters()
        {
            // استخدام _context بدل context
            var crafters = _context.Users
                .Include(x => x.Role) // لو مش Include هيعمل NullReference
                .Where(x => x.Role.Name == "Crafter")
                .Select(u => new CraftersViewModel
                {
                    FullName = u.FullName,
                    PhoneNumber = u.PhoneNumber,
                    ProfilePic = u.ProfilePic // Use stored profile picture path
                })
               .ToList();

            return View(crafters);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
