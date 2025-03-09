using CraftingServiceApp.Domain.Entities;
using CraftingServiceApp.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CraftingServiceApp.AdminAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public UserController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpDelete("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            var userPosts = _context.Posts.Where(p => p.ClientId == user.Id);
            _context.Posts.RemoveRange(userPosts);

            var userRequests = _context.Requests.Where(r => r.ClientId == user.Id || r.CrafterId == user.Id);
            _context.Requests.RemoveRange(userRequests);

            var userServices = _context.Services.Where(s => s.UserId == user.Id);
            _context.Services.RemoveRange(userServices);

            var userAddresses = _context.Addresses.Where(a => a.UserId == user.Id);
            _context.Addresses.RemoveRange(userAddresses);

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                await _context.SaveChangesAsync();
                return Ok(new { Message = "User deleted successfully" });
            }

            return BadRequest(new { Message = "Error occurred while deleting the user", Errors = result.Errors });
        }
    }
}
