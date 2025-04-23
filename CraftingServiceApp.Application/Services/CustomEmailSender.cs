using CraftingServiceApp.BLL.Interfaces;
using CraftingServiceApp.Infrastructure.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace CraftingServiceApp.BLL.Services
{
    public class CustomEmailSender : ICustomEmailSender
    {
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;

        public CustomEmailSender(IEmailSender emailSender, ApplicationDbContext context)
        {
            _emailSender = emailSender;
            _context = context;
        }

        public async Task SendPaymentHeldNotificationAsync(string userId, int paymentId)
        {
            var user = await _context.Users.FindAsync(userId);
            var payment = await _context.Payments.FindAsync(paymentId);

            var subject = "Payment Held Notification";
            var message = $"Hello {user.UserName}, your payment of {payment.Amount} is being held.";

            await _emailSender.SendEmailAsync(user.Email, subject, message);
        }

        public async Task SendRequestCompletedNotificationAsync(string userId, int requestId)
        {
            var user = await _context.Users.FindAsync(userId);
            var request = await _context.Requests
                .Include(r => r.Service)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (user != null && request != null)
            {
                var subject = "Request Completed";
                var message = $"Your request for {request.Service.Title} has been marked as completed. " +
                             $"The payment of {request.Payment?.Amount} has been released to the crafter.";

                await _emailSender.SendEmailAsync(user.Email, subject, message);
            }
        }

        public async Task SendDisputeNotificationAsync(string clientId, string crafterId, int requestId)
        {
            // Get all required data in one query
            var request = await _context.Requests
                .Include(r => r.Service)
                .Include(r => r.Client)
                .Include(r => r.Payment)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null) return;

            // Notify admin
            var adminEmails = await (
                from user in _context.Users
                join userRole in _context.UserRoles on user.Id equals userRole.UserId
                join role in _context.Roles on userRole.RoleId equals role.Id
                where role.Name == "Admin"
                select user.Email
            ).ToListAsync();

            var adminSubject = $"Dispute Reported - Request #{requestId}";
            var adminMessage = $"""
            A dispute has been reported for request #{requestId}.
            
            Client: {request.Client?.UserName} ({request.Client?.Email})
            Service: {request.Service?.Title}
            Amount: {request.Payment?.Amount}
            
            Please review the dispute and take appropriate action.
            """;

            foreach (var email in adminEmails)
            {
                await _emailSender.SendEmailAsync(email, adminSubject, adminMessage);
            }

            // Notify crafter
            if (!string.IsNullOrEmpty(crafterId))
            {
                var crafter = await _context.Users.FindAsync(crafterId);
                if (crafter != null)
                {
                    var crafterSubject = $"Dispute Reported - Your Service Request";
                    var crafterMessage = $"""
                    A dispute has been reported for your service "{request.Service?.Title}".
                    
                    Request ID: #{requestId}
                    Client: {request.Client?.UserName}
                    Amount: {request.Payment?.Amount}
                    
                    The admin team will review this dispute and contact you if needed.
                    """;

                    await _emailSender.SendEmailAsync(crafter.Email, crafterSubject, crafterMessage);
                }
            }
        }


        // Implement other IEmailSender methods
        public Task SendEmailAsync(string email, string subject, string message)
        {
            return _emailSender.SendEmailAsync(email, subject, message);
        }
    }
}
