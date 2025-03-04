using CraftingServiceApp.Application.Interfaces;
using CraftingServiceApp.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CraftingServiceApp.Web.Controllers
{
    public class ServicesController : Controller
    {
        private readonly IRepository<Service> _ServiceRepository;
        private readonly IRepository<Category> _CategoRyrepository;

        public ServicesController(IRepository<Service> serviceRepository, IRepository<Category> categoRyrepository)
        {
            _ServiceRepository = serviceRepository;
            _CategoRyrepository = categoRyrepository;
        }

        // Index: Display all services
        public IActionResult Index()
        {
            var services = _ServiceRepository.GetAll();
            ViewData["Categories"] = new SelectList(_CategoRyrepository.GetAll(), "Id", "Name");
            return View(services);
        }

        // Details: Display a single service
        public IActionResult Details(int id)
        {
            var service = _ServiceRepository.GetById(id);
            ViewData["CategoryId"] = new SelectList(_CategoRyrepository.GetAll(), "Id", "Name");
            if (service == null)
            {
                return BadRequest();
            }
            return View(service);
        }

        [Authorize]
        // Create: Display the form for creating a new service
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_CategoRyrepository.GetAll(), "Id", "Name");
            return View();
        }

        [Authorize]
        // Create: Handle form submission for creating a new service
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Service service)
        {
            //if (!ModelState.IsValid)
            //{
            //    ViewData["CategoryId"] = new SelectList(_CategoRyrepository.GetAll(), "Id", "Name");
            //    return View(service);
            //}

            if (service.ImageFile != null)
            {
                // Define the uploads folder (ensure `wwwroot/uploads` exists)
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                // Generate a unique file name
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + service.ImageFile.FileName;

                // Combine path and filename
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save the file to wwwroot/uploads
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await service.ImageFile.CopyToAsync(fileStream);
                }

                // Store the relative path in the database
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
