using CraftingServiceApp.AdminAPI.Dtos;
using CraftingServiceApp.AdminAPI.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CraftingServiceApp.Web.Controllers
{
    public class AdminTicketsController : Controller
    {
        private readonly string apiUrl = "https://localhost:7293/api/ticket/";

        public IActionResult Index()
        {
            return View(); // Will show all tickets using a view
        }

        public async Task<IActionResult> Details(int id)
        {
            // Fetch ticket details using the API's GetTicket method
            var ticket = await GetTicket(id);

            if (ticket == null)
            {
                return NotFound("Ticket not found");
            }

            ViewBag.Ticket = ticket;
            return View(ticket);
        }

        private async Task<TicketDto> GetTicket(int id)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"{apiUrl}{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ContentContainer<TicketDto>>(json);
                    return result?.Data;
                }
            }
            return null;
        }
    }


}
