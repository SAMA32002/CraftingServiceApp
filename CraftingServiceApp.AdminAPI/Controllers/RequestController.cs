using CraftingServiceApp.AdminAPI.Dtos;
using CraftingServiceApp.Domain.Entities;
using CraftingServiceApp.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CraftingServiceApp.AdminAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RequestController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAllRequests")]
        public async Task<IActionResult> GetAllRequests()
        {
            // Fetch all requests
            var requests = await _context.Requests
                .ToListAsync();

            if (requests == null || !requests.Any())
            {
                return NotFound(new { Message = "No requests found" });
            }

            // Map the requests to the DTO format
            var requestList = requests.Select(r => new RequestDTO
            {
                Id = r.Id,
                ClientId = r.ClientId,  // Only the ID
                ServiceId = r.ServiceId,  // Only the ID
                RequestDate = r.RequestDate,
                Status = r.Status.ToString(),  // Use enum value as a string (optional)
                SelectedScheduleId = r.SelectedScheduleId,  // Only the ID
                ScheduledDateTime = r.ScheduledDateTime,
                Notes = r.Notes,
                PaymentStatus = r.PaymentStatus.ToString(),  // Use enum value as a string (optional)
                ProposedDates = r.ProposedDates.Select(ps => ps.Id).ToList(),  // List of IDs
                SelectedAddressId = r.SelectedAddressId,  // Only the ID
                PaymentIntentId = r.PaymentIntentId
            }).ToList();

            // Get the total count of requests
            var totalCount = requests.Count();

            return Ok(new { TotalCount = totalCount, Requests = requestList });
        }
    }

}
