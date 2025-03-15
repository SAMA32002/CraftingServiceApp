using CraftingServiceApp.Application.Interfaces;
using CraftingServiceApp.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CraftingServiceApp.Web.Controllers
{
    public class CraftersController : Controller
    {
        private IUserRepository _userRepository;
        public CraftersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public IActionResult Index()
        {
            // استخدام _context بدل context
            var crafters = _userRepository.GetAll()
                .Include(x => x.Role) // لو مش Include هيعمل NullReference
                .Where(x => x.Role.Name == "Crafter")
                .Select(u => new CraftersViewModel
                {
                    FullName = u.FullName,
                    PhoneNumber = u.PhoneNumber,
                    ProfilePic = u.ProfilePic // Use stored profile picture path
                })
               .ToList();

            return View(crafters);
        }

    }
}
