using CraftingServiceApp.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace CraftingServiceApp.Application.Interfaces
{
    public class PaymentService : IPaymentService
    {
        private readonly IRepository<Payment> _paymentRepository;

        public PaymentService(IRepository<Payment> paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public void ProcessPayment(Payment payment)
        {
            _paymentRepository.Add(payment);
            _paymentRepository.SaveChanges();
        }

        public void RefundPayment(int paymentId)
        {
            var payment = _paymentRepository.GetById(paymentId);
            if (payment != null)
            {
                payment.Status = PaymentStatus.Refunded;
                _paymentRepository.Update(payment);
                _paymentRepository.SaveChanges();
            }
        }
    }

}
