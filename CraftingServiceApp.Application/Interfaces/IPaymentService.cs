using CraftingServiceApp.Domain.Entities;
using System.Threading.Tasks;

namespace CraftingServiceApp.Application.Interfaces
{
    public interface IPaymentService
    {
      Task<Payment?> CreateOrUpdatePaymentIntentId(string UserId, int ServiceId, decimal Price);

       Task UpdatePaymentIntentStatus(string PaymentIntentId, bool isSuccess);
    }
}
