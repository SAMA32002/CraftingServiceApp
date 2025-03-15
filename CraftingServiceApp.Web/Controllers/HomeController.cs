using CraftingServiceApp.Application.Interfaces;
using CraftingServiceApp.Infrastructure.Data;
using CraftingServiceApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


namespace CraftingServiceApp.Web.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private IUserRepository _userRepository;

        public HomeController(ApplicationDbContext context, IUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
