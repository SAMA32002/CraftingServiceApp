
using Stripe;

namespace CraftingServiceApp.BLL.Interfaces
{
    public interface IStripeService
    {
        Task<PaymentIntent> CreatePaymentIntentAsync(decimal amount, string currency);
        Task<PaymentIntent> CapturePaymentAsync(string paymentIntentId);
        Task<Refund> CreateRefundAsync(string paymentIntentId);
    }
}
