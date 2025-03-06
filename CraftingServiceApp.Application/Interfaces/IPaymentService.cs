using CraftingServiceApp.Domain.Entities;

namespace CraftingServiceApp.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<UserPayment?> CreateOrUpdatePaymentIntent(string userId, int serviceId, decimal price); // Fix serviceId type
        Task<bool> UpdatePaymentStatus(string paymentIntentId, bool isSuccess);
    }
}
