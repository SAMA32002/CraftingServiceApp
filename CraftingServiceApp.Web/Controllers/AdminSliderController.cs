using CraftingServiceApp.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CraftingServiceApp.Web.Controllers
{
    public class AdminSliderController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _environment;

        public AdminSliderController(IHttpClientFactory httpClientFactory, IWebHostEnvironment environment)
        {
            _httpClientFactory = httpClientFactory;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://localhost:7293/api/SliderItem/GetAllSliderItems");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to load slider items.";
                return View(new List<SliderItemViewModel>());
            }

            var content = await response.Content.ReadAsStringAsync();
            var items = JsonConvert.DeserializeObject<List<SliderItemViewModel>>(content);

            return View(items);
        }
    }

}
