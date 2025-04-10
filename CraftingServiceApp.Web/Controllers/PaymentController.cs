using CraftingServiceApp.Application.Interfaces;
using CraftingServiceApp.Domain.DTOs;
using CraftingServiceApp.Domain.Entities;
using CraftingServiceApp.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Stripe;


namespace CraftingServiceApp.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task<ActionResult> Index()
        {
            var result = await _paymentService.GetAllPaymentsAsync();
            return View(result.Data); // You can also pass the message to ViewBag if needed
        }

        public async Task<ActionResult> Details(int id)
        {
            var result = await _paymentService.GetPaymentByIdAsync(id);
            if (result.Data == null) return NotFound();

            return View(result.Data);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreatePaymentRequestDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _paymentService.CreatePaymentAsync(dto);
            if (!result.Data.IsSuccess)
                ModelState.AddModelError("", result.Message);

            return RedirectToAction("Details", new { id = result.Data.PaymentId });
        }

        [HttpPost]
        public async Task<ActionResult> UpdateStatus(int id, PaymentStatus status, bool isSuccess)
        {
            var result = await _paymentService.UpdatePaymentStatusAsync(id, status, isSuccess);
            TempData["Message"] = result.Message;
            return RedirectToAction("Details", new { id });
        }
    }
}
