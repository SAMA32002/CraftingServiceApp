using CraftingServiceApp.Application.Interfaces;
using CraftingServiceApp.Domain.Entities;
using CraftingServiceApp.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CraftingServiceApp.AdminAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IRepository<Service> _serviceRepository;
        private readonly ApplicationDbContext _context;

        public ServiceController(IRepository<Service> serviceRepository, ApplicationDbContext context)
        {
            _serviceRepository = serviceRepository;
            _context = context;
        }

        [HttpDelete("DeleteService/{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var service = await _context.Services
                .Include(s => s.Reviews)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (service == null)
            {
                return NotFound(new { Message = "Service not found" });
            }

            if (service.Reviews != null && service.Reviews.Any())
            {
                _context.Reviews.RemoveRange(service.Reviews);
            }

            _serviceRepository.Delete(service);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Service deleted successfully" });
        }
    }
}
