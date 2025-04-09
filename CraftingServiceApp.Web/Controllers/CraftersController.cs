using CraftingServiceApp.Application.Interfaces;
using CraftingServiceApp.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CraftingServiceApp.Web.Controllers
{
    public class CraftersController : Controller
    {
        private readonly IUserRepository _userRepository;

        public CraftersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public IActionResult Index()
        {
            var crafters = _userRepository.GetAll()
                .Include(x => x.Role)
                .Where(x => x.Role.Name == "Crafter")
                .Select(u => new CraftersViewModel
                {
                    Id = u.Id, // ✅ تأكد من إضافة الـ Id هنا
                    FullName = u.FullName,
                    PhoneNumber = u.PhoneNumber,
                    ProfilePic = u.ProfilePic
                })
                .ToList();

            return View(crafters);
        }

        // ✅ أكشن صفحة البروفايل
        public IActionResult Profile(string id)
        {
            var crafter = _userRepository.GetAll()
                .Include(x => x.Role)
                .Where(x => x.Role.Name == "Crafter" && x.Id == id)
                .Select(u => new CraftersViewModel
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    PhoneNumber = u.PhoneNumber,
                    ProfilePic = u.ProfilePic
                })
                .FirstOrDefault();

            if (crafter == null)
                return NotFound();

            return View(crafter);
        }
    }
}
