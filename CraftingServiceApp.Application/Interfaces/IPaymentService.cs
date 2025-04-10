using CraftingServiceApp.Domain.DTOs;
using CraftingServiceApp.Domain.Entities;
using CraftingServiceApp.Domain.Enums;
using CraftingServiceApp.Domain.Helper;
using System.Threading.Tasks;

namespace CraftingServiceApp.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<ContentContainer<CreatePaymentResponseDto>> CreatePaymentAsync(CreatePaymentRequestDto dto);
        Task<ContentContainer<PaymentDetailsDto>> GetPaymentByIdAsync(int id);
        Task<ContentContainer<List<PaymentDetailsDto>>> GetAllPaymentsAsync();
        Task<ContentContainer<string>> UpdatePaymentStatusAsync(int paymentId, PaymentStatus newStatus, bool isSuccess);
    }
}
