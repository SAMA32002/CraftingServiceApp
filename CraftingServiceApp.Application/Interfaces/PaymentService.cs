using CraftingServiceApp.Application.Interfaces;
using CraftingServiceApp.Domain.Entities;
using CraftingServiceApp.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Stripe;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CraftingServiceApp.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IRepository<UserPayment> _userPaymentRepository;

        public PaymentService(IConfiguration configuration, IRepository<UserPayment> userPaymentRepository)
        {
            _configuration = configuration;
            _userPaymentRepository = userPaymentRepository;
        }

        public async Task<UserPayment?> CreateOrUpdatePaymentIntentId(string UserId, int ServiceId, decimal Price)
        {
            // Set Stripe API key
            StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];

            // Here, Price is already provided, so we'll calculate the total amount
            var amount = (long)(Price * 100); // Convert to cents

            // Create a PaymentIntentService instance
            var service = new PaymentIntentService();
            PaymentIntent paymentIntent;

            // Retrieve existing UserPayment for the given UserId and ServiceId using Find()
            var userPayment = await _userPaymentRepository
                .Find(up => up.UserId == UserId && up.ServiceId == ServiceId) // Apply filter with Find()
                .FirstOrDefaultAsync(); // Use FirstOrDefaultAsync() to retrieve the first result asynchronously

            if (userPayment == null) // If no payment exists, create new payment intent
            {
                var options = new PaymentIntentCreateOptions()
                {
                    Amount = amount, // Amount in cents
                    Currency = "usd",
                    PaymentMethodTypes = new List<string>() { "card" }
                };

                paymentIntent = await service.CreateAsync(options);

                // Create a new UserPayment record
                userPayment = new UserPayment
                {
                    UserId = UserId,
                    ServiceId = ServiceId,
                    PaymentId = paymentIntent.Id,
                    ClientSecret = paymentIntent.ClientSecret,
                    Amount = Price,
                    Status = PaymentStatus.Pending
                };

                // Save the new UserPayment entity using the generic repository
                await _userPaymentRepository.AddAsync(userPayment);
                await _userPaymentRepository.SaveAsync(); // Save changes to the database
            }
            else // If a payment exists, update the payment intent
            {
                var options = new PaymentIntentUpdateOptions()
                {
                    Amount = amount, // Update amount if needed
                };

                paymentIntent = await service.UpdateAsync(userPayment.PaymentId, options);

                userPayment.PaymentId = paymentIntent.Id;
                userPayment.ClientSecret = paymentIntent.ClientSecret;

                // Save updated UserPayment entity
                _userPaymentRepository.Update(userPayment); // Mark entity as modified for update
                await _userPaymentRepository.SaveAsync();
            }

            return userPayment;
        }

        public async Task UpdatePaymentIntentStatus(string PaymentIntentId, bool isSuccess)
        {
            // Retrieve the existing payment record by PaymentIntentId
            var userPayment = await _userPaymentRepository
                .Find(up => up.PaymentId == PaymentIntentId)
                .FirstOrDefaultAsync(); // Find by PaymentId

            if (userPayment != null)
            {
                // Update the payment status based on the result
                userPayment.Status = isSuccess ? PaymentStatus.Completed : PaymentStatus.Failed;
                userPayment.IsSuccess = isSuccess;

                // Save the updated status
                 _userPaymentRepository.Update(userPayment);
                await _userPaymentRepository.SaveAsync(); // Save changes to the database
            }
        }
    }
}
