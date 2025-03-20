using CraftingServiceApp.Application.Interfaces;
using CraftingServiceApp.Domain.Entities;
using CraftingServiceApp.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Stripe;
using System.Threading.Tasks;

namespace CraftingServiceApp.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IConfiguration _configuration;
        private readonly IRepository<UserPayment> _userPaymentRepository;

        // Inject the necessary services
        public PaymentController(IPaymentService paymentService,
                                 IConfiguration configuration,
                                  IRepository<UserPayment> userPaymentRepository)
        {
            _paymentService = paymentService;
            _configuration = configuration;
            _userPaymentRepository = userPaymentRepository;
        }

        // API endpoint to create or update payment intent for a user based on ServiceId and amount
        [HttpPost("CreateOrUpdatePaymentIntent/{UserId}/{ServiceId}")]
        public async Task<IActionResult> CreateOrUpdatePaymentIntent(string UserId, int ServiceId, decimal Amount)
        {
            var userPayment = await _paymentService.CreateOrUpdatePaymentIntentId(UserId, ServiceId, Amount);
            if (userPayment == null)
            {
                return BadRequest(new { message = "There is a problem with your payment information!" });
            }

            // Return the UserPayment with PaymentId and ClientSecret
            return Ok(new { PaymentId = userPayment.PaymentId, ClientSecret = userPayment.ClientSecret });
        }

        // Stripe Webhook endpoint to handle payment status updates from Stripe
        [HttpPost("Webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new System.IO.StreamReader(Request.Body).ReadToEndAsync();
            var stripeSignature = Request.Headers["Stripe-Signature"];

            if (string.IsNullOrEmpty(stripeSignature))
            {
                return BadRequest(new { message = "Missing Stripe signature" });
            }

            // Construct the event from the incoming Stripe webhook payload
            var stripeEvent = Stripe.EventUtility.ConstructEvent(json, stripeSignature, _configuration["StripeSettings:WebhookSecret"]);

            // Handle successful payment
            if (stripeEvent.Type == "payment_intent.succeeded")
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                if (paymentIntent != null)
                {
                    // Update payment status in your repository (assuming the service layer updates the database)
                    await _paymentService.UpdatePaymentIntentStatus(paymentIntent.Id, true);
                }
            }
            // Handle failed payment
            else if (stripeEvent.Type == "payment_intent.failed")
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                if (paymentIntent != null)
                {
                    await _paymentService.UpdatePaymentIntentStatus(paymentIntent.Id, false);
                }
            }

            return Ok();
        }
    }
}
