using Microsoft.AspNetCore.Mvc;

namespace CraftingServiceApp.Controllers
{
    public class AdminUsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Edit(int id)
        {
            ViewBag.UserId = id;
            return View();
        }
    }
}
