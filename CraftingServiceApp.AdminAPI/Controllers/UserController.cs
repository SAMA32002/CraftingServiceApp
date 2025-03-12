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
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
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

            var userRequests = _context.Requests.Where(r => r.ClientId == user.Id );
            _context.Requests.RemoveRange(userRequests);

            var userServices = _context.Services.Where(s => s.CrafterId == user.Id);
            _context.Services.RemoveRange(userServices);

            var userAddresses = _context.Address.Where(a => a.ClientId == user.Id);
            _context.Address.RemoveRange(userAddresses);

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                await _context.SaveChangesAsync();
                return Ok(new { Message = "User deleted successfully" });
            }

            return BadRequest(new { Message = "Error occurred while deleting the user", Errors = result.Errors });
        }


            [HttpPut("BanUser/{id}")]
            public async Task<IActionResult> BanUser(string id)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { Message = "User not found" });
                }

                user.IsBanned = true;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return Ok(new { Message = "User has been banned successfully." });
                }

                return BadRequest(new { Message = "Error occurred while banning the user", Errors = result.Errors });
            }

            [HttpPut("UnbanUser/{id}")]
            public async Task<IActionResult> UnbanUser(string id)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { Message = "User not found" });
                }

                user.IsBanned = false;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return Ok(new { Message = "User has been unbanned successfully." });
                }

                return BadRequest(new { Message = "Error occurred while unbanning the user", Errors = result.Errors });
            }

            [HttpPut("UpdateUserRole/{id}")]
            public async Task<IActionResult> UpdateUserRole(string id, [FromBody] string newRole)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { Message = "User not found" });
                }

                var roleExists = await _roleManager.RoleExistsAsync(newRole);
                if (!roleExists)
                {
                    return BadRequest(new { Message = $"Role '{newRole}' does not exist" });
                }

                var currentRoles = await _userManager.GetRolesAsync(user);
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    return BadRequest(new { Message = "Error occurred while removing the user from their current roles" });
                }

                var addResult = await _userManager.AddToRoleAsync(user, newRole);
                if (!addResult.Succeeded)
                {
                    return BadRequest(new { Message = "Error occurred while adding the user to the new role", Errors = addResult.Errors });
                }

                return Ok(new { Message = $"User role updated to {newRole}" });
            }

}
}
