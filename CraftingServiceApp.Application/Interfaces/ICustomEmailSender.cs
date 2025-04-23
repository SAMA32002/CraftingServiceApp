
using Microsoft.AspNetCore.Identity.UI.Services;

namespace CraftingServiceApp.BLL.Interfaces
{
    public interface ICustomEmailSender : IEmailSender
    {
        Task SendPaymentHeldNotificationAsync(string userId, int paymentId);
        Task SendRequestCompletedNotificationAsync(string userId, int requestId);
        Task SendDisputeNotificationAsync(string clientId, string crafterId, int requestId);
    }
}
