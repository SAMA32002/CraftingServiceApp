using CraftingServiceApp.Application.Interfaces;
using CraftingServiceApp.Domain.Entities;
using CraftingServiceApp.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CraftingServiceApp.Web.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Notification> _notificationRepository;

        public NotificationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IRepository<Notification> notificationRepository)
        {
            _context = context;
            _userManager = userManager;
            _notificationRepository = notificationRepository;
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = _notificationRepository.GetById(id);
            //var notification = await _context.Notifications.FindAsync(id);
            if (notification == null || notification.UserId != _userManager.GetUserId(User))
            {
                return NotFound();
            }

            notification.IsRead = true;
            await _notificationRepository.SaveAsync();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> MarkAllRead()
        {
            var userId = _userManager.GetUserId(User);
            var notifications = _notificationRepository.GetAll()
                .Where(n => n.UserId == userId && !n.IsRead);

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await _notificationRepository.SaveAsync();

            return Ok();
        }
    }

}
