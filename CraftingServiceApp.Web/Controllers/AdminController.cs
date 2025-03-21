using Microsoft.AspNetCore.Mvc;

namespace CraftingServiceApp.Web.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
