using CraftingServiceApp.Domain.Entities;
using CraftingServiceApp.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CraftingServiceApp.Web.Controllers
{   
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult AdminLogin(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminLogin(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (!isAdmin)
            {
                ModelState.AddModelError(string.Empty, "Access denied. You are not authorized.");
                return View(model);
            }

            // Log in
            await HttpContext.SignInAsync(
                scheme: "AdminScheme",
                principal: await _signInManager.CreateUserPrincipalAsync(user),
                properties: new AuthenticationProperties { IsPersistent = model.RememberMe }
            );

            return RedirectToLocal(returnUrl);
        }

        [HttpGet]
        [Route("Admin/AdminLogout")]
        public async Task<IActionResult> AdminLogout()
        {
            await HttpContext.SignOutAsync("AdminScheme");
            HttpContext.Response.Cookies.Delete("AdminScheme");
            return RedirectToAction("AdminLogin", "Admin");
        }


        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Admin");
        }


        [Authorize(AuthenticationSchemes = "AdminScheme", Roles = "Admin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
