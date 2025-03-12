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

        public ServicesController(
            UserManager<ApplicationUser> userManager,
            IRepository<Service> serviceRepository,
            IRepository<Category> categoRyrepository,
            IRepository<Review> reviewRepository,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _ServiceRepository = serviceRepository;
            _CategoRyrepository = categoRyrepository;
            _ReviewRepository = reviewRepository;
            _context = context;
        }

        // عرض جميع الخدمات
        public IActionResult Index(int? categoryId)
        {
            var services = _context.Services.Include(s => s.Crafter).ToList();

            ViewData["Categories"] = new SelectList(_CategoRyrepository.GetAll(), "Id", "Name");

            if (categoryId.HasValue)
            {
                services = services.Where(s => s.CategoryId == categoryId.Value).ToList();
            }

            return View(services);
        }

        // تفاصيل الخدمة
        public async Task<IActionResult> DetailsAsync(int id)
        {
            var service = await _context.Services
                .Include(s => s.Reviews)
                .ThenInclude(r => r.Client)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (service == null)
                return BadRequest();

            var reviews = service.Reviews ?? new List<Review>();
            var avgRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;

            var viewModel = new ServiceDetailsViewModel
            {
                Service = service,
                AverageRating = avgRating,
                Review = new Review()
            };

            return View(viewModel);
        }

        // إضافة تقييم للخدمة
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddReview(Review review)
        {
            review.ClientId = _userManager.GetUserId(User);

            ModelState.Clear();
            TryValidateModel(review);

            if (!ModelState.IsValid)
            {
                return RedirectToAction("Details", new { id = review.ServiceId });
            }

            _ReviewRepository.Add(review);
            _ReviewRepository.SaveChanges();

            return RedirectToAction("Details", new { id = review.ServiceId });
        }

        // عرض الفورم لإنشاء خدمة جديدة
        [Authorize(Roles = "Crafter")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_CategoRyrepository.GetAll(), "Id", "Name");
            return View();
        }

        // إضافة خدمة جديدة
        [Authorize(Roles = "Crafter")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Service service)
        {
            service.CrafterId = _userManager.GetUserId(User);

            ModelState.Clear();
            TryValidateModel(service);

            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = new SelectList(_CategoRyrepository.GetAll(), "Id", "Name");
                return BadRequest(new { message = "Invalid data" });
            }

            if (service.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                Directory.CreateDirectory(uploadsFolder);

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

            return Json(new { success = true, message = "Service added successfully!" });
        }

        // ✅ عرض نموذج تعديل الخدمة
        [Authorize(Roles = "Crafter")]
        public IActionResult Edit(int id)
        {
            var service = _context.Services
                .Include(s => s.Crafter)
                .Include(s => s.Category)
                .FirstOrDefault(s => s.Id == id);

            if (service == null)
                return NotFound();

            // التحقق من أن المستخدم الحالي هو صاحب الخدمة أو أدمن
            if (service.CrafterId != _userManager.GetUserId(User) && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            ViewData["CategoryId"] = new SelectList(_CategoRyrepository.GetAll(), "Id", "Name", service.CategoryId);

            return View(service);
        }

        // ✅ تعديل الخدمة - POST
        [Authorize(Roles = "Crafter")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Service service)
        {
            if (id != service.Id)
                return NotFound();

            // جلب الخدمة الأصلية للتحقق والتعديل
            var originalService = _ServiceRepository.GetById(id);
            if (originalService == null)
                return NotFound();

            if (originalService.CrafterId != _userManager.GetUserId(User) && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // التعامل مع رفع الصورة الجديدة
                    if (service.ImageFile != null)
                    {
                        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                        Directory.CreateDirectory(uploadsFolder);

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + service.ImageFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await service.ImageFile.CopyToAsync(fileStream);
                        }

                        // حذف الصورة القديمة لو مش الافتراضية
                        if (!string.IsNullOrEmpty(originalService.Image) && !originalService.Image.Contains("default"))
                        {
                            string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot",
                                originalService.Image.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                            if (System.IO.File.Exists(oldImagePath))
                            {
                                try
                                {
                                    System.IO.File.Delete(oldImagePath);
                                }
                                catch
                                {
                                    // تجاهل أي خطأ في الحذف
                                }
                            }
                        }

                        service.Image = "/uploads/" + uniqueFileName;
                    }
                    else
                    {
                        // لو مفيش صورة جديدة، احتفظ بالقديمة
                        service.Image = originalService.Image;
                    }

                    // تعديل بيانات الخدمة
                    originalService.Title = service.Title;
                    originalService.Description = service.Description;
                    originalService.Price = service.Price;
                    originalService.CategoryId = service.CategoryId;
                    originalService.Image = service.Image;

                    _ServiceRepository.Update(originalService);
                    _ServiceRepository.SaveChanges();

                    TempData["SuccessMessage"] = "تم تعديل الخدمة بنجاح!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Services.Any(e => e.Id == service.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewData["CategoryId"] = new SelectList(_CategoRyrepository.GetAll(), "Id", "Name", service.CategoryId);
            return View(service);
        }

        // عرض صفحة الحذف
        [Authorize]
        public IActionResult Delete(int id)
        {
            var service = _ServiceRepository.GetById(id);

            if (service == null)
                return NotFound();

            return View(service);
        }

        // تنفيذ الحذف
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
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
