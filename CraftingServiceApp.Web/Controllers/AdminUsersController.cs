using Microsoft.AspNetCore.Mvc;

namespace CraftingServiceApp.Controllers
{
    public class AdminUsersController : Controller
    {
        private readonly IConfiguration _configuration;

        public AdminUsersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Edit(string id)
        {
            ViewBag.UserId = id; // Pass User ID to Edit View
            return View();
        }
    }
}
