using CraftingServiceApp.Application.Interfaces;
using CraftingServiceApp.Infrastructure.Data;
using CraftingServiceApp.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CraftingServiceApp.Web.Controllers
{
    public class CraftersController : Controller
    {
        private readonly IUserRepository _userRepository;

        public ApplicationDbContext Context { get; }

        public CraftersController(IUserRepository userRepository , ApplicationDbContext context)
        {
            _userRepository = userRepository;
            Context = context;
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
        public IActionResult CrafterProfileDetails(string id)
        {
            var crafter = _userRepository.GetById(id); // Replace with your method
            return PartialView("_CrafterProfileDetails", crafter);
        }
        public IActionResult ServiceDetails(int id)
        {
            var service = Context.Services
                .Where(s => s.Id == id)
                .Select(s => new ServiceDetailViewModel
                {
                    Id = s.Id,
                    Title = s.Title,
                    Description = s.Description,
                    Price = s.Price
                    // Add any other fields that you want to show in the service details
                }).FirstOrDefault();

            if (service == null)
            {
                return NotFound(); // Or handle error as needed
            }

            return PartialView("ServiceDetails", service);
        }

        public IActionResult CrafterServices(string id)
        {
            var services = Context.Services
           .Where(s => s.CrafterId == id)
           .Select(s => new ServiceViewModel
           {
           Id = s.Id,
           Title = s.Title
           // Add other properties you want to show
           }).ToList();

            return PartialView("_CrafterServices", services);
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
