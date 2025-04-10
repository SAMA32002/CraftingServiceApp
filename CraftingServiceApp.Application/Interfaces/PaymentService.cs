using CraftingServiceApp.Application.Interfaces;
using CraftingServiceApp.Domain.Entities;
using CraftingServiceApp.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Stripe;
using Microsoft.EntityFrameworkCore;
using CraftingServiceApp.Domain.DTOs;
using CraftingServiceApp.Domain.Helper;
using CraftingServiceApp.Infrastructure.Data;

namespace CraftingServiceApp.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;

        public PaymentService(IConfiguration config, ApplicationDbContext context)
        {
            _config = config;
            _context = context;
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
        }

        public async Task<ContentContainer<CreatePaymentResponseDto>> CreatePaymentAsync(CreatePaymentRequestDto dto)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(dto.Amount * 100),
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" },
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            var payment = new Payment
            {
                ClientId = dto.ClientId,
                CrafterId = dto.CrafterId,
                RequestId = dto.RequestId,
                ServiceId = dto.ServiceId,
                Amount = dto.Amount,
                PaymentIntentId = paymentIntent.Id,
                ClientSecret = paymentIntent.ClientSecret,
                Status = PaymentStatus.Pending,
                IsSuccess = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            var response = new CreatePaymentResponseDto
            {
                PaymentId = payment.Id,
                PaymentIntentId = payment.PaymentIntentId,
                ClientSecret = payment.ClientSecret,
                IsSuccess = true
            };

            return new ContentContainer<CreatePaymentResponseDto>(response, "Payment created successfully");
        }

        public async Task<ContentContainer<PaymentDetailsDto>> GetPaymentByIdAsync(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
                return new ContentContainer<PaymentDetailsDto>(null, "Payment not found");

            var dto = new PaymentDetailsDto
            {
                PaymentId = payment.Id,
                ClientId = payment.ClientId,
                CrafterId = payment.CrafterId,
                Amount = payment.Amount,
                Status = payment.Status.ToString(),
                IsSuccess = payment.IsSuccess,
                CreatedAt = payment.CreatedAt
            };

            return new ContentContainer<PaymentDetailsDto>(dto, "Payment found");
        }

        public async Task<ContentContainer<List<PaymentDetailsDto>>> GetAllPaymentsAsync()
        {
            var data = await _context.Payments
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new PaymentDetailsDto
                {
                    PaymentId = p.Id,
                    ClientId = p.ClientId,
                    CrafterId = p.CrafterId,
                    Amount = p.Amount,
                    Status = p.Status.ToString(),
                    IsSuccess = p.IsSuccess,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();

            return new ContentContainer<List<PaymentDetailsDto>>(data, "All payments retrieved");
        }

        public async Task<ContentContainer<string>> UpdatePaymentStatusAsync(int paymentId, PaymentStatus newStatus, bool isSuccess)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null)
                return new ContentContainer<string>(null, "Payment not found");

            payment.Status = newStatus;
            payment.IsSuccess = isSuccess;
            await _context.SaveChangesAsync();

            return new ContentContainer<string>("Updated", "Status updated successfully");
        }
    }
}
