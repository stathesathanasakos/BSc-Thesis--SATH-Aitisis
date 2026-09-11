using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SATH.Aitisis.App.Common.Constants;
using SATH.Aitisis.App.Services.Payments;
//using SATH.Aitisis.Domain.Constants;
using SATH.Aitisis.Web.Areas.Citizen.ViewModels.Payments;
using System.Security.Claims;

namespace SATH.Aitisis.Web.Areas.Citizen.Controllers
{
    [Area("Citizen")]
    [Authorize(Roles = AppRoles.Citizen)]
    public sealed class PaymentsController : Controller
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<IActionResult> Details(int applicationId)
        {
            var citizenId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(citizenId))
            {
                return Unauthorized();
            }

            var details = await _paymentService.GetPaymentDetailsAsync(applicationId, citizenId);

            if (details == null)
            {
                return NotFound();
            }

            var model = new PaymentDetailsViewModel
            {
                ApplicationId = details.ApplicationId,
                ApplicationTypeName = details.ApplicationTypeName,
                Amount = details.Amount,
                Currency = details.Currency,
                RequiresPayment = details.RequiresPayment,
                CanStartPayment = details.CanStartPayment,
                PaymentStatusText = details.PaymentStatus?.ToString()
                    ?? "Not started"
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Start(int applicationId)
        {
            var citizenId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(citizenId))
            {
                return Unauthorized();
            }

            var successBaseUrl = Url.Action(
                nameof(Success),
                "Payments",
                new { area = "Citizen" },
                Request.Scheme);

            var cancelUrl = Url.Action(
                nameof(Cancel),
                "Payments",
                new
                {
                    area = "Citizen",
                    applicationId
                },
                Request.Scheme);

            if (string.IsNullOrWhiteSpace(successBaseUrl) || string.IsNullOrWhiteSpace(cancelUrl))
            {
                TempData["ErrorMessage"] = "Payment redirect URLs could not be created.";

                return RedirectToAction(nameof(Details), new { applicationId });
            }

            var successUrl = $"{successBaseUrl}?sessionId={{CHECKOUT_SESSION_ID}}";

            var result =
                await _paymentService.StartPaymentAsync(
                    new StartPaymentRequest
                    {
                        ApplicationId = applicationId,
                        CitizenId = citizenId,
                        SuccessUrl = successUrl,
                        CancelUrl = cancelUrl
                    });

            if (!result.Succeeded || string.IsNullOrWhiteSpace(result.CheckoutUrl))
            {
                TempData["ErrorMessage"] = result.ErrorMessage ?? "The payment could not be started.";

                return RedirectToAction(nameof(Details), new { applicationId });
            }

            return Redirect(result.CheckoutUrl);
        }

        [HttpGet]
        public async Task<IActionResult> Success(string sessionId)
        {
            var citizenId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(citizenId))
            {
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(sessionId))
            {
                TempData["ErrorMessage"] = "Payment session id was not provided.";
                return RedirectToAction("MyApplications", "Applications", new { area = "Citizen" });
            }

            var result =
                await _paymentService.CompletePaymentAsync(
                    new CompletePaymentRequest
                    {
                        ExternalSessionId = sessionId,
                        CitizenId = citizenId
                    });

            if (!result.Succeeded || !result.ApplicationId.HasValue)
            {
                TempData["ErrorMessage"] = result.ErrorMessage ?? "The payment could not be confirmed.";
                return RedirectToAction("MyApplications", "Applications", new { area = "Citizen" });
            }

            TempData["SuccessMessage"] = "The payment was completed successfully.";

            return View(result.ApplicationId.Value);
        }

        [HttpGet]
        public IActionResult Cancel(int applicationId)
        {
            TempData["ErrorMessage"] = "The payment was cancelled before completion.";

            return View(applicationId);
        }
    }
}