using CraftingServiceApp.Domain.Entities;

namespace CraftingServiceApp.Application.Interfaces
{
    public interface IPaymentService
    {
        void ProcessPayment(Payment payment);
        void RefundPayment(int paymentId);
    }
}
