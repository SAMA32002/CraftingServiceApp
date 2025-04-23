using System.Security.Claims;
using CraftingServiceApp.BLL.Interfaces;
using CraftingServiceApp.Domain.Entities;
using CraftingServiceApp.Domain.Enums;
using CraftingServiceApp.Infrastructure.Data;
using CraftingServiceApp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;


namespace CraftingServiceApp.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IStripeService _stripeService;
        private readonly ApplicationDbContext _context;
        private readonly ICustomEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(
            IStripeService stripeService,
            ApplicationDbContext context,
            ICustomEmailSender emailSender,
            IConfiguration configuration,
            ILogger<PaymentController> logger)
        {
            _stripeService = stripeService;
            _context = context;
            _emailSender = emailSender;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int requestId)
        {
            var request = await _context.Requests
                .Include(r => r.Service)
                .Include(r => r.Client)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null || !request.IsApproved || request.ClientId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();
            }

            var viewModel = new PaymentViewModel
            {
                RequestId = request.Id,
                Amount = request.TotalAmount,
                ServiceName = request.Service.Title,
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PaymentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var request = await _context.Requests
                .Include(r => r.Service)
                .ThenInclude(s => s.Crafter)
                .Include(r => r.Client)
                .FirstOrDefaultAsync(r => r.Id == model.RequestId);

            if (request == null || request.Service?.CrafterId == null ||
                !request.IsApproved || request.ClientId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();
            }

            try
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = Convert.ToInt64(model.Amount * 100), // Convert to cents
                    Currency = "egp", // Must be lowercase
                    PaymentMethodTypes = new List<string> { "card" }, // Required
                    CaptureMethod = "manual" // If you're holding funds
                };

                var service = new PaymentIntentService();
                var paymentIntent = await service.CreateAsync(options);

                // Log the created PaymentIntent
                _logger.LogInformation("Created PaymentIntent: {Id}", paymentIntent.Id);

                var payment = new Payment
                {
                    StripePaymentIntentId = paymentIntent.Id,
                    StripeClientSecret = paymentIntent.ClientSecret,
                    Amount = model.Amount,
                    RequestId = request.Id,
                    ClientId = request.ClientId,
                    CrafterId = request.Service.CrafterId,
                    Status = PaymentStatus.Pending
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                return RedirectToAction("Process", new { paymentId = payment.Id });
            }
            catch (StripeException e)
            {
                _logger.LogError(e, "Stripe error");
                ModelState.AddModelError("", "Payment service error. Please try again.");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Process(int paymentId)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.Id == paymentId && p.Status == PaymentStatus.Pending);

            if (payment == null) return NotFound();

            ViewBag.StripePublishableKey = _configuration["Stripe:PublishableKey"];
            return View(new ProcessPaymentViewModel
            {
                PaymentId = payment.Id,
                ClientSecret = payment.StripeClientSecret,
                Amount = payment.Amount,
                RequestId = payment.RequestId
            });
        }

        // Handle successful payments
        [HttpGet]
        public IActionResult Success(int paymentId)
        {
            var payment = _context.Payments.Find(paymentId);
            return View(payment); // Show success message
        }

        // Handle failed payments
        [HttpGet]
        public IActionResult Failure(int paymentId)
        {
            return View(); // Show error message
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    _configuration["Stripe:WebhookSecret"]
                );

                if (stripeEvent.Type == "payment_intent.succeeded")
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    var payment = await _context.Payments
                        .Include(p => p.Crafter)
                        .FirstOrDefaultAsync(p => p.StripePaymentIntentId == paymentIntent.Id);

                    if (payment != null)
                    {
                        payment.Status = PaymentStatus.Held;
                        await _context.SaveChangesAsync();
                        await _emailSender.SendPaymentHeldNotificationAsync(payment.CrafterId, payment.Id);
                    }
                }

                return Ok();
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe webhook error");
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> SimulateWebhook(int paymentId)
        {
            // Find the payment and include the related Request
            var payment = await _context.Payments
                .Include(p => p.Request) // Make sure to include the Request
                .FirstOrDefaultAsync(p => p.Id == paymentId);

            if (payment != null)
            {
                payment.Status = PaymentStatus.Succeeded;

                // Update the Request's PaymentId if it's not already set
                if (payment.Request != null && payment.Request.PaymentId == null)
                {
                    payment.Request.PaymentId = paymentId;
                }

                await _context.SaveChangesAsync();
            }
            return Ok();
        }
    }
}
