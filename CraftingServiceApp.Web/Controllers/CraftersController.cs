using CraftingServiceApp.Application.Interfaces;
using CraftingServiceApp.Domain.Entities;
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
                .Include(s => s.Reviews)
                .Include(s => s.Crafter)
                .Include(s => s.Category)
                .FirstOrDefault(s => s.Id == id);

            if (service == null)
            {
                return NotFound();
            }

            var reviews = service.Reviews ?? new List<Review>();
            var avgRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;

            var viewModel = new ServiceDetailsViewModel
            {
                Service = service,
                AverageRating = avgRating,
                Review = new Review() // optional, if you're using it
            };

            return PartialView("ServiceDetails", viewModel);
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
