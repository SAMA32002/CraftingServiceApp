using CraftingServiceApp.Infrastructure.Data;
using CraftingServiceApp.Web.Models;
using CraftingServiceApp.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CraftingServiceApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }


        // عرض الصفحة الرئيسية مع آخر الخدمات
        public async Task<IActionResult> Index()
        {
            // Fetch recent services
            var recentServices = await _context.Services
                .Include(s => s.Category)
                .Include(s => s.Crafter)
                .OrderByDescending(s => s.Id)
                .Take(4)
                .ToListAsync();

            // Fetch active slider images from the API
            var httpClient = _httpClientFactory.CreateClient();
            var sliderApiUrl = "https://localhost:7293/api/SliderItem/GetAllSliderItems"; // Update if you have a different route
            var allSliders = await httpClient.GetFromJsonAsync<List<SliderItemViewModel>>(sliderApiUrl);
            var activeSliders = allSliders?.Where(s => s.IsActive).ToList() ?? new List<SliderItemViewModel>();

            // Combine both into a single view model
            var viewModel = new HomePageViewModel
            {
                RecentServices = recentServices,
                ActiveSliders = activeSliders
            };

            return View(viewModel);
        }


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