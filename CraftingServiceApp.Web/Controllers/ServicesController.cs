using CraftingServiceApp.Application.Interfaces;
using CraftingServiceApp.Domain.Entities;
using CraftingServiceApp.Infrastructure.Data;
using CraftingServiceApp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CraftingServiceApp.Web.Controllers
{
    public class ServicesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Service> _ServiceRepository;
        private readonly IRepository<Category> _CategoRyrepository;
        private readonly IRepository<Review> _ReviewRepository;
        private readonly ApplicationDbContext _context;

        public ServicesController(UserManager<ApplicationUser> userManager, IRepository<Service> serviceRepository, IRepository<Category> categoRyrepository, IRepository<Review> reviewRepository, ApplicationDbContext context)
        {
            _userManager = userManager;
            _ServiceRepository = serviceRepository;
            _CategoRyrepository = categoRyrepository;
            _ReviewRepository = reviewRepository;
            _context = context;
        }


        // Index: Display all services
        //public IActionResult Index(int? categoryId)
        //{
        //    var services = _ServiceRepository.GetAll();
        //    ViewData["Categories"] = new SelectList(_CategoRyrepository.GetAll(), "Id", "Name");

        //    if (categoryId.HasValue)
        //    {
        //        services = _ServicesService.GetServicesByCategory(categoryId.Value);
        //    }

        //    return View(services);
        //}
        public IActionResult Index(int? categoryId)
        {
            var services = _context.Services.Include(s => s.Crafter).ToList(); // Ensure Crafter is included
            ViewData["Categories"] = new SelectList(_CategoRyrepository.GetAll(), "Id", "Name");

            if (categoryId.HasValue)
            {
                services = services.Where(s => s.CategoryId == categoryId.Value).ToList();
            }

            return View(services);
        }

        // Details: Display a single service
        public async Task<IActionResult> DetailsAsync(int id)
        {
            //var service = _ServiceRepository.GetById(id);
            var service = await _context.Services
                .Include(s => s.Reviews)
                .ThenInclude(r => r.Client) // Ensures Client details are loaded
                .FirstOrDefaultAsync(s => s.Id == id);

            ViewData["CategoryId"] = new SelectList(_CategoRyrepository.GetAll(), "Id", "Name");
            if (service == null)
            {
                return BadRequest();
            }

            // Ensure Reviews list is not null
            var reviews = service.Reviews ?? new List<Review>();

            // Calculate Average Rating
            var avgRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;

            var viewModel = new ServiceDetailsViewModel
            {
                Service = service,
                AverageRating = avgRating,
                Review = new Review() // Initialize empty review model
            };

            return View(viewModel);
            //return View(service);
        }

        [HttpPost]
        [Authorize] // Ensure only logged-in users can submit reviews
        public async Task<IActionResult> AddReview(Review review)
        {
            review.ClientId = _userManager.GetUserId(User); // Get current user ID

            ModelState.Clear();
            TryValidateModel(review);

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                      .Select(e => e.ErrorMessage)
                                      .ToList();
                return RedirectToAction("Details", new { id = review.ServiceId });
            }

            _ReviewRepository.Add(review);
            _ReviewRepository.SaveChanges();

            return RedirectToAction("Details", new { id = review.ServiceId });
        }


        // Create: Display the form for creating a new service
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_CategoRyrepository.GetAll(), "Id", "Name");
            return View();
        }

        // Create: Handle form submission for creating a new service
        [Authorize(Roles = "Crafter")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Service service)
        {
            service.CrafterId = _userManager.GetUserId(User); // Get current user ID

            ModelState.Clear();
            TryValidateModel(service);

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                      .Select(e => e.ErrorMessage)
                                      .ToList();
                ViewData["CategoryId"] = new SelectList(_CategoRyrepository.GetAll(), "Id", "Name");
                return RedirectToAction(nameof(Index));
            }

            if (service.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + service.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await service.ImageFile.CopyToAsync(fileStream);
                }

                service.Image = "/uploads/" + uniqueFileName;
            }

            _ServiceRepository.Add(service);
            _ServiceRepository.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        [Authorize]
        // Edit: Display the form for editing an existing service
        public async Task<IActionResult> Edit(int id)
        {
            var service = _ServiceRepository.GetById(id);
            ViewData["CategoryId"] = new SelectList(_CategoRyrepository.GetAll(), "Id", "Name");
            if (service == null)
            {
                return NotFound();
            }
            return View(service);
        }

        [Authorize]
        // Edit: Handle form submission for updating an existing service
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Service service)
        {
            if (id != service.Id)
            {
                return BadRequest();
            }

            //if (!ModelState.IsValid)
            //{
            //    return View(service);
            //}

            _ServiceRepository.Update(service);
            _ServiceRepository.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        // Delete: Display confirmation page for deleting a service
        public async Task<IActionResult> Delete(int id)
        {
            var service = _ServiceRepository.GetById(id);
            if (service == null)
            {
                return NotFound();
            }
            return View(service);
        }

        [Authorize]
        // Delete: Handle deletion of a service
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var service = _ServiceRepository.GetById(id);
            if (service != null)
            {
                _ServiceRepository.Delete(service);
                _ServiceRepository.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
