using CraftingServiceApp.Domain.Entities;
using System.Threading.Tasks;

namespace CraftingServiceApp.Application.Interfaces
{
    public interface IPaymentService
    {
      Task<UserPayment?> CreateOrUpdatePaymentIntentId(string UserId, int ServiceId, decimal Price);

       Task UpdatePaymentIntentStatus(string PaymentIntentId, bool isSuccess);
    }
}
