using CraftingServiceApp.Application.Interfaces;
using CraftingServiceApp.Domain.Entities;
using CraftingServiceApp.Infrastructure.Data;
using CraftingServiceApp.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CraftingServiceApp.Web.Controllers
{
    public class TicketController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("submit")]
        public IActionResult SubmitTicket()
        {
            return View();
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitTicket(TicketViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var ticket = new Ticket
            {
                Email = model.Email,
                Subject = model.Subject,
                Message = model.Message,
                CreatedAt = DateTime.UtcNow,
                Status = TicketStatus.Open
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your ticket has been submitted successfully!";
            return RedirectToAction("SubmitTicket");
        }
    }
}


