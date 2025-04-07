using CraftingServiceApp.Domain.Entities;
using CraftingServiceApp.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using CraftingServiceApp.Infrastructure.Data;
using CraftingServiceApp.Application.Interfaces;

namespace CraftingServiceApp.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public UsersController(IUserRepository userRepository, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userRepository.GetAll()
                .Include(u => u.Services) 
                .Include(u => u.Addresses)
                .Include(u => u.SentRequests)
                .ThenInclude(r => r.Service) 
                .ThenInclude(s => s.Crafter) 
                .Include(u => u.ReceivedRequests)
                .ThenInclude(r => r.Client) 
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            // Get user's roles
            var roles = await _userManager.GetRolesAsync(user);
            bool isCrafter = roles.Contains("Crafter");

            var viewModel = new ProfileViewModel
            {
                User = user,
                IsCrafter = isCrafter,
                Services = user.Services?.ToList() ?? new List<Service>(),
                ReceivedRequests = user.ReceivedRequests?.ToList() ?? new List<Request>(),
                SentRequests = user.SentRequests?.ToList() ?? new List<Request>(),
                Posts = _context.Posts.Where(p => p.ClientId == userId).ToList(),
                Addresses = user.Addresses?.ToList() ?? new List<Address>()
            };

            return View(viewModel);
        }

        public async Task<IActionResult> RegisterAsync()
        {
            var allowedRoles = new List<string> { "Crafter", "Client" };
            var roles = await _roleManager.Roles
                                          .Where(r => allowedRoles.Contains(r.Name))
                                          .ToListAsync();

            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserRegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(model.RoleId);
                if (role == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid role selected.");
                    return View(model);
                }
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    RoleId = role.Id
                };

                if (model.ProfilePicture != null)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfilePicture.FileName;

                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ProfilePicture.CopyToAsync(fileStream);
                    }

                    user.ProfilePic = "/uploads/" + uniqueFileName;
                }

                // Add addresses to the user
                foreach (var addressViewModel in model.Addresses)
                {
                    user.Addresses.Add(new Address
                    {
                        Street = addressViewModel.Street,
                        City = addressViewModel.City,
                        PostalCode = addressViewModel.PostalCode,
                        Country = "Egypt"
                    });
                }

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Assign Role to User
                    var roleResult = await _userManager.AddToRoleAsync(user, role.Name);
                    if (!roleResult.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "Failed to assign role.");
                        return View(model);
                    }
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // تسجيل الدخول (GET)
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Find the user by email
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            // Verify password
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!isPasswordValid)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            // Sign the user in
            await _signInManager.SignInAsync(user, model.RememberMe);

           
            // Redirect to home or returnUrl if valid
            return RedirectToLocal(returnUrl);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        // تسجيل الخروج
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.Users
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

            if (user == null)
            {
                return NotFound();
            }

            var model = new EditProfileViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ExistingProfilePicture = user.ProfilePic,
                Addresses = user.Addresses.Select(a => new AddressViewModel
                {
                    Id = a.Id,
                    Street = a.Street,
                    City = a.City,
                    PostalCode = a.PostalCode,
                    //Country = a.Country,
                    IsPrimary = false,
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            user.FullName = model.FullName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;

            // Update password if provided
            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                if (!passwordResult.Succeeded)
                {
                    foreach (var error in passwordResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
            }

            if (model.ProfilePicture != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.ProfilePicture.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                if (!string.IsNullOrEmpty(user.ProfilePic))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.ProfilePic.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfilePicture.CopyToAsync(fileStream);
                }

                user.ProfilePic = "/uploads/" + uniqueFileName;
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Profile));
            }

            ModelState.AddModelError("", "Error updating profile.");
            return View(model);
        }

    }
}
