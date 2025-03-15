//using CraftingServiceApp.BLL.Interfaces;
//using CraftingServiceApp.Domain.Entities;
//using Microsoft.Extensions.Configuration;
//using Stripe;

//namespace CraftingServiceApp.Application.Interfaces
//{
//    public class PaymentService : IPaymentService
//    {
//        private readonly IConfiguration _configuration;
//        private readonly IUnitOfWork<UserPayment> _unitOfWork;
//        private readonly IUnitOfWork<UserPayment> _ServiceunitOfWork;

//        public PaymentService(IConfiguration configuration, IUnitOfWork<UserPayment> unitOfWork, IUnitOfWork<UserPayment> serviceunitOfWork)
//        {
//            _configuration = configuration;
//            _unitOfWork = unitOfWork;
//            _ServiceunitOfWork = serviceunitOfWork;
//            StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];
//        }



//        public async Task<UserPayment?> CreateOrUpdatePaymentIntent(string userId, int serviceId, decimal price)
//        {
//            var service = await _ServiceunitOfWork.GetByIdAsync(serviceId);
//            if (service is null)
//                return null;

//            var userPayment = await _unitOfWork
//                                    .FindAsync(up => up.UserId == userId && up.ServiceId == serviceId);

//            var paymentService = new PaymentIntentService();
//            PaymentIntent paymentIntent;

//            if (userPayment == null)
//            {
//                var options = new PaymentIntentCreateOptions
//                {
//                    Amount = (long)(price * 100),
//                    Currency = "usd",
//                    PaymentMethodTypes = new List<string> { "card" },
//                    Metadata = new Dictionary<string, string>
//                {
//                    { "ServiceId", serviceId.ToString() },
//                    { "UserId", userId }
//                }
//                };
//                paymentIntent = await paymentService.CreateAsync(options);

//                userPayment = new UserPayment
//                {
//                    UserId = userId,
//                    ServiceId = serviceId,
//                    PaymentId = paymentIntent.Id,
//                    ClientSecret = paymentIntent.ClientSecret,
//                    Amount = price
//                };

//                await _unitOfWork.AddAsync(userPayment);
//            }
//            else
//            {
//                var options = new PaymentIntentUpdateOptions
//                {
//                    Amount = (long)(price * 100)
//                };
//                paymentIntent = await paymentService.UpdateAsync(userPayment.PaymentId, options);

//                userPayment.PaymentId = paymentIntent.Id;
//                userPayment.ClientSecret = paymentIntent.ClientSecret;
//                userPayment.Amount = price;

//                await _unitOfWork.UpdateAsync(userPayment);
//            }

//            await _unitOfWork.CompleteAsync();
//            return userPayment;
//        }

//        public async Task<bool> UpdatePaymentStatus(string paymentIntentId, bool isSuccess)
//        {
//            var userPayment = await _unitOfWork.FindAsync(up => up.PaymentId == paymentIntentId);

//            if (userPayment == null)
//                return false;

//            userPayment.IsSuccess = isSuccess;
//            await _unitOfWork.UpdateAsync(userPayment);
//            await _unitOfWork.CompleteAsync();

//            return true;
//        }
//    }

//}
