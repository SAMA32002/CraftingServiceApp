using CraftingServiceApp.Application.Interfaces;
using CraftingServiceApp.Domain.Entities;
using CraftingServiceApp.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Stripe;
using Microsoft.EntityFrameworkCore;

namespace CraftingServiceApp.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IRepository<Payment> _PaymentRepository;

        public PaymentService(IConfiguration configuration, IRepository<Payment> PaymentRepository)
        {
            _configuration = configuration;
            _PaymentRepository = PaymentRepository;
        }

        public async Task<Payment?> CreateOrUpdatePaymentIntentId(string UserId, int ServiceId, decimal Price)
        {
            // Set Stripe API key
            StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];

            // Here, Price is already provided, so we'll calculate the total amount
            var amount = (long)(Price * 100); // Convert to cents

            // Create a PaymentIntentService instance
            var service = new PaymentIntentService();
            PaymentIntent paymentIntent;

            // Retrieve existing Payment for the given UserId and ServiceId using Find()
            var Payment = await _PaymentRepository
                .Find(up => up.ClientId == UserId && up.ServiceId == ServiceId) // Apply filter with Find()
                .FirstOrDefaultAsync(); // Use FirstOrDefaultAsync() to retrieve the first result asynchronously

            if (Payment == null) // If no payment exists, create new payment intent
            {
                var options = new PaymentIntentCreateOptions()
                {
                    Amount = amount, // Amount in cents
                    Currency = "usd",
                    PaymentMethodTypes = new List<string>() { "card" }
                };

                paymentIntent = await service.CreateAsync(options);

                // Create a new Payment record
                Payment = new Payment
                {
                    ClientId = UserId,
                    ServiceId = ServiceId,
                    PaymentIntentId = paymentIntent.Id,
                    ClientSecret = paymentIntent.ClientSecret,
                    Amount = Price,
                    Status = PaymentStatus.Pending
                };

                // Save the new Payment entity using the generic repository
                await _PaymentRepository.AddAsync(Payment);
                await _PaymentRepository.SaveAsync(); // Save changes to the database
            }
            else // If a payment exists, update the payment intent
            {
                var options = new PaymentIntentUpdateOptions()
                {
                    Amount = amount, // Update amount if needed
                };

                paymentIntent = await service.UpdateAsync(Payment.PaymentIntentId, options);

                Payment.PaymentIntentId = paymentIntent.Id;
                Payment.ClientSecret = paymentIntent.ClientSecret;

                // Save updated Payment entity
                _PaymentRepository.Update(Payment); // Mark entity as modified for update
                await _PaymentRepository.SaveAsync();
            }

            return Payment;
        }

        public async Task UpdatePaymentIntentStatus(string PaymentIntentId, bool isSuccess)
        {
            // Retrieve the existing payment record by PaymentIntentId
            var Payment = await _PaymentRepository
                .Find(up => up.PaymentIntentId == PaymentIntentId)
                .FirstOrDefaultAsync(); // Find by PaymentId

            if (Payment != null)
            {
                // Update the payment status based on the result
                Payment.Status = isSuccess ? PaymentStatus.Completed : PaymentStatus.Failed;
                Payment.IsSuccess = isSuccess;

                // Save the updated status
                 _PaymentRepository.Update(Payment);
                await _PaymentRepository.SaveAsync(); // Save changes to the database
            }
        }
    }
}
