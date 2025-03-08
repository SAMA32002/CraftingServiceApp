using CraftingServiceApp.Domain.Enums;

namespace CraftingServiceApp.Domain.Entities
{
    public class UserPayment
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }  // المستخدم الذي يقوم بالدفع
        public int ServiceId { get; set; }  // الخدمة التي يتم شراؤها
        public int RequestId { get; set; }  // Link to request
        public string PaymentId { get; set; }  // معرف الدفع من Stripe
        public string ClientSecret { get; set; }  // المفتاح السري للدفع
        public decimal Amount { get; set; }  // السعر الذي تم دفعه
        public bool IsSuccess { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    }
}
