using System.Security.Claims;
using CraftingServiceApp.Application.Interfaces;
using CraftingServiceApp.BLL.Interfaces;
using CraftingServiceApp.BLL.Services;
using CraftingServiceApp.Domain.Entities;
using CraftingServiceApp.Domain.Enums;
using CraftingServiceApp.Infrastructure.Data;
using CraftingServiceApp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;

namespace CraftingServiceApp.Web.Controllers
{
    [Authorize]
    public class RequestController : Controller
    {
        private readonly IRepository<Request> _requestRepository;
        private readonly IRepository<Domain.Entities.Address> _addressRepository;
        private readonly IRepository<Domain.Entities.Service> _serviceRepository;
        private readonly IRepository<Notification> _notificationRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOptions<StripeSettings> _stripeSettings;
        private readonly ICustomEmailSender _emailSender;
        private readonly ILogger<RequestController> _logger;
        private readonly IStripeService _stripeService;

        public RequestController(IRepository<Request> requestRepository, IRepository<Domain.Entities.Address> addressRepository, IRepository<Domain.Entities.Service> serviceRepository, IRepository<Notification> notificationRepository, ApplicationDbContext context, UserManager<ApplicationUser> userManager, IOptions<StripeSettings> stripeSettings, ICustomEmailSender emailSender, ILogger<RequestController> logger, IStripeService stripeService)
        {
            _requestRepository = requestRepository;
            _addressRepository = addressRepository;
            _serviceRepository = serviceRepository;
            _notificationRepository = notificationRepository;
            _context = context;
            _userManager = userManager;
            _stripeSettings = stripeSettings;
            _emailSender = emailSender;
            _logger = logger;
            _stripeService = stripeService;
        }

        // GET: Request/Create
        public async Task<IActionResult> Create(int serviceId)
        {
            var service = _serviceRepository.GetById(serviceId);
            if (service == null)
            {
                return NotFound();
            }

            var client = await _userManager.GetUserAsync(User);
            if (client == null)
            {
                return Unauthorized();
            }

            // Load client's saved addresses
            var clientAddresses = await _addressRepository.GetAll()
                .Where(a => a.ClientId == client.Id)
                .ToListAsync();

            var viewModel = new RequestCreateViewModel
            {
                ServiceId = serviceId,
                ServiceTitle = service.Title,
                ClientAddresses = clientAddresses
            };

            return View(viewModel);
        }

        // POST: Request/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RequestCreateViewModel model)
        {
            var client = await _userManager.GetUserAsync(User);
            if (client == null)
            {
                return Unauthorized();
            }

            int? selectedAddressId = model.SelectedAddressId;

            // If the client choose to enter a new address, save it first
            if (!model.UseExistingAddress)
            {
                if (!string.IsNullOrWhiteSpace(model.NewStreet) && !string.IsNullOrWhiteSpace(model.NewCity))
                {
                    var newAddress = new Domain.Entities.Address
                    {
                        ClientId = client.Id,
                        Street = model.NewStreet,
                        City = model.NewCity,
                        PostalCode = model.NewPostalCode,
                        Country = model.NewCountry,
                        IsPrimary = false
                    };

                    _addressRepository.Add(newAddress);
                    await _addressRepository.SaveAsync();
                    selectedAddressId = newAddress.Id; // Use the new address for the request
                }
            }

            var request = new Request
            {
                ClientId = client.Id,
                ServiceId = model.ServiceId,
                Status = RequestStatus.Pending,
                RequestDate = DateTime.UtcNow,
                Notes = model.Notes,
                SelectedAddressId = selectedAddressId, // Assign selected address

                // Assign custom address fields
                CustomStreet = !model.UseExistingAddress ? model.NewStreet : null,
                CustomCity = !model.UseExistingAddress ? model.NewCity : null,
                CustomPostalCode = !model.UseExistingAddress ? model.NewPostalCode : null,
                CustomCountry = !model.UseExistingAddress ? model.NewCountry : null
            };


            _requestRepository.Add(request);
            await _requestRepository.SaveAsync();

            var schedules = new List<RequestSchedule>();

            if (model.ProposedDate1 != default(DateTime) && model.ProposedDate1 > DateTime.UtcNow)
            {
                schedules.Add(new RequestSchedule { RequestId = request.Id, ProposedDate = model.ProposedDate1 });
            }
            if (model.ProposedDate2.HasValue && model.ProposedDate2.Value > DateTime.UtcNow)
            {
                schedules.Add(new RequestSchedule { RequestId = request.Id, ProposedDate = model.ProposedDate2.Value });
            }
            if (model.ProposedDate3.HasValue && model.ProposedDate3.Value > DateTime.UtcNow)
            {
                schedules.Add(new RequestSchedule { RequestId = request.Id, ProposedDate = model.ProposedDate3.Value });
            }

            if (schedules.Any())
            {
                _context.requestSchedules.AddRange(schedules);
                await _context.SaveChangesAsync();
            }

            // Send Notification to Crafter
            var service = _serviceRepository.GetById(model.ServiceId);
            if (service != null)
            {
                var notification = new Notification
                {
                    UserId = service.CrafterId,
                    Message = $"You have received a new request from {client.UserName}.",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                _notificationRepository.Add(notification);
                await _notificationRepository.SaveAsync();
            }

            return RedirectToAction("Details", "Request", new { id = request.Id });
        }

        public async Task<IActionResult> Details(int id)
        {
            var request = await _requestRepository.GetAll()
                .Include(r => r.Service)
                .Include(r => r.ProposedDates)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            var viewModel = new RequestDetailsViewModel
            {
                ServiceTitle = request.Service.Title,
                Status = request.Status,
                RequestDate = request.RequestDate,
                Notes = request.Notes,
                ProposedDates = request.ProposedDates.Select(d => d.ProposedDate).ToList()
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Crafter")]
        public async Task<IActionResult> ReceivedRequests()
        {
            var userId = _userManager.GetUserId(User);
            var requests = await _requestRepository.GetAll()
                .Where(r => r.Service.CrafterId == userId)
                .Include(r => r.Client)
                .Include(r => r.Service)
                .Include(r => r.ProposedDates) // Load available schedule dates
                .ToListAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ReceivedRequestsPartial", requests);
            }
            return View(requests);
        }

        [Authorize(Roles = "Client")]
        public async Task<IActionResult> SentRequests()
        {
            var userId = _userManager.GetUserId(User); // Get the logged-in client ID
            var sentRequests = await _requestRepository.GetAll()
                .Where(r => r.ClientId == userId)
                .Include(r => r.Service)
                .Include(r => r.SelectedSchedule)
                .Include(r => r.SelectedAddress)                
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_SentRequestsPartial", sentRequests);
            }
            return View(sentRequests);
        }

        [HttpPost]
        [Authorize(Roles = "Crafter")]
        public async Task<IActionResult> AcceptRequest(int requestId, int selectedScheduleId)
        {
            var request = await _requestRepository.GetAll()
                .Include(r => r.Service)
                    .ThenInclude(s => s.Crafter)
                .Include(r => r.Client)
                .Include(r => r.ProposedDates)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null || request.Service?.CrafterId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();
            }

            var selectedSchedule = request.ProposedDates.FirstOrDefault(s => s.Id == selectedScheduleId);
            if (selectedSchedule == null)
            {
                return BadRequest("Invalid schedule selection.");
            }

            // Update request status
            request.SelectedScheduleId = selectedScheduleId;
            request.Status = RequestStatus.Accepted;
            request.ScheduledDateTime = selectedSchedule.ProposedDate;
            request.TotalAmount = request.Service.Price;
            request.IsApproved = true; // Flag for payment workflow
            await _requestRepository.SaveAsync();

            // Create notification without link
            var notification = new Notification
            {
                UserId = request.ClientId,
                Message = $"Your request for {request.Service?.Title} has been accepted. " +
                         "Please visit your requests to complete payment.",
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };
            _notificationRepository.Add(notification);
            await _notificationRepository.SaveAsync();

            return RedirectToAction("ReceivedRequests");
        }

        [HttpPost]
        public async Task<IActionResult> DeclineRequest(int requestId)
        {
            var request = await _requestRepository.GetAll()
                .Include(r => r.Service) // ✅ Include Service
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null) return NotFound();

            request.Status = RequestStatus.Rejected;

            var notification = new Notification
            {
                UserId = request.ClientId,
                Message = $"Your request for {request.Service?.Title} was declined.", // ✅ Null-safe
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };
            _notificationRepository.Add(notification);

            await _notificationRepository.SaveAsync();
            return RedirectToAction(nameof(ReceivedRequests));
        }

        [HttpPost]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> MarkAsCompleted(int requestId)
        {
            var request = await _context.Requests
                .Include(r => r.Payment)
                .Include(r => r.Service)
                    .ThenInclude(s => s.Crafter) // Get crafter through service
                .Include(r => r.Client)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null || request.Payment == null || request.Service?.Crafter == null)
            {
                return NotFound();
            }

            try
            {
                request.IsCompleted = true;

                if (request.Payment.Status == PaymentStatus.Held)
                {
                    await _stripeService.CapturePaymentAsync(request.Payment.StripePaymentIntentId);
                    request.Payment.Status = PaymentStatus.Released;
                    request.Payment.ReleasedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                // Notify client
                await _emailSender.SendRequestCompletedNotificationAsync(request.ClientId, request.Id);

                // Notify crafter (through service)
                await _emailSender.SendEmailAsync(
                    request.Service.Crafter.Email,
                    "Payment Released",
                    $"Your payment of {request.Payment.Amount} for {request.Service.Title} has been released.");

                return RedirectToAction("Details", new { id = requestId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking request {RequestId} as completed", requestId);
                return RedirectToAction("Details", new { id = requestId, error = "Failed to complete request" });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DisputeRequest(int requestId)
        {
            var request = await _context.Requests
                .Include(r => r.Service)
                .Include(r => r.Payment)
                .FirstOrDefaultAsync(r => r.Id == requestId && r.ClientId == User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (request == null)
            {
                return NotFound();
            }

            request.IsDisputed = true;

            if (request.Payment?.Status == PaymentStatus.Held)
            {
                await _stripeService.CreateRefundAsync(request.Payment.StripePaymentIntentId);
                request.Payment.Status = PaymentStatus.Refunded;
                request.Payment.DisputedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            // Send notifications
            await _emailSender.SendDisputeNotificationAsync(
                request.ClientId,
                request.Service?.CrafterId,
                request.Id);

            return RedirectToAction("Details", new { id = requestId });
        }
    }

}
