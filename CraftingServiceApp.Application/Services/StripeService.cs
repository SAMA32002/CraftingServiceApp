using CraftingServiceApp.BLL.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace CraftingServiceApp.BLL.Services
{
    public class StripeService : IStripeService
    {
        private readonly ChargeService _chargeService;
        private readonly PaymentIntentService _paymentIntentService;
        private readonly RefundService _refundService;

        public StripeService(IConfiguration configuration)
        {
            StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];
            _chargeService = new ChargeService();
            _paymentIntentService = new PaymentIntentService();
            _refundService = new RefundService();
        }

        public async Task<PaymentIntent> CreatePaymentIntentAsync(decimal amount, string currency)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100), // Stripe uses smallest currency unit
                Currency = currency,
                CaptureMethod = "manual", // Important for holding funds
            };

            return await _paymentIntentService.CreateAsync(options);
        }

        public async Task<PaymentIntent> CapturePaymentAsync(string paymentIntentId)
        {
            return await _paymentIntentService.CaptureAsync(paymentIntentId);
        }

        public async Task<Refund> CreateRefundAsync(string paymentIntentId)
        {
            var options = new RefundCreateOptions
            {
                PaymentIntent = paymentIntentId
            };

            return await _refundService.CreateAsync(options);
        }
    }
}
