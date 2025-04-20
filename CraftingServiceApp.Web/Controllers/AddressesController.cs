using CraftingServiceApp.Domain.Entities;
using CraftingServiceApp.Infrastructure.Data;
using CraftingServiceApp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CraftingServiceApp.Web.Controllers
{
    [Authorize]
    public class AddressesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AddressesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddressViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var address = new Address
            {
                ClientId = user.Id,
                Street = model.Street,
                City = model.City,
                PostalCode = model.PostalCode,
                //Country = model.Country,
                IsPrimary = false
            };
            _context.Address.Add(address);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Address address)
        {
            if (ModelState.IsValid)
            {
                var existingAddress = await _context.Address
                    .Where(a => a.Id == address.Id && !a.IsDeleted)
                    .FirstOrDefaultAsync();

                if (existingAddress == null)
                {
                    return NotFound();
                }

                // Update address fields
                existingAddress.Street = address.Street;
                existingAddress.City = address.City;
                existingAddress.PostalCode = address.PostalCode;
                existingAddress.IsPrimary = address.IsPrimary;

                _context.Update(existingAddress);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var address = await _context.Address.FindAsync(id);
            var userId = _userManager.GetUserId(User);

            if (address == null)
            {
                return Json(new { success = false, message = "Address not found" });
            }

            if (userId == null || address.ClientId != userId)
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            try
            {
                address.IsDeleted = true;
                _context.Update(address);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetPrimary(int id)
        {
            var userId = _userManager.GetUserId(User);
            var addresses = await _context.Address
                .Where(a => a.ClientId == userId)
                .ToListAsync();

            foreach (var addr in addresses)
                addr.IsPrimary = (addr.Id == id);

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
    }

}
