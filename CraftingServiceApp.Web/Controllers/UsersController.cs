using CraftingServiceApp.Domain.Entities;
using CraftingServiceApp.Domain.Enums;
using CraftingServiceApp.Web.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CraftingServiceApp.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
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
                // Create User object
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
                    // Define the uploads folder (ensure `wwwroot/uploads` exists)
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                    // Generate a unique file name
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfilePicture.FileName;

                    // Combine path and filename
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save the file to wwwroot/uploads
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ProfilePicture.CopyToAsync(fileStream);
                    }

                    // Store the relative path in the database
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
                    });
                }

                // Save User and Addresses
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

        // Helper method to handle redirects
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
    }
}
