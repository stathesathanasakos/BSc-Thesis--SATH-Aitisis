using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SATH.Aitisis.App.Common.Constants;
using SATH.Aitisis.App.Services.Applications;
using SATH.Aitisis.Web.Areas.Employee.ViewModels.Applications;
using System.Security.Claims;
using SATH.Aitisis.Web.ViewModels.Applications;
using SATH.Aitisis.App.Services.AiReview;



namespace SATH.Aitisis.Web.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Roles = AppRoles.Employee)]
    public class ApplicationsController : Controller
    {
        private readonly IApplicationService _applicationService;
        private readonly IApplicationAiReviewService _applicationAiReviewService;

        public ApplicationsController(IApplicationService applicationService, IApplicationAiReviewService applicationAiReviewService)
        {
            _applicationService = applicationService;
            _applicationAiReviewService = applicationAiReviewService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var applications = await _applicationService.GetPendingApplicationsAsync();

            var model = applications
                .Select(x => new PendingApplicationListItemViewModel
                {
                    Id = x.Id,
                    ApplicationTypeName = x.ApplicationTypeName,
                    SubmissionDate = x.SubmissionDate,
                    StatusText = "Pending"
                })
                .ToList();

            return View(model);
        }



        // to review the details of a specific application
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var application = await _applicationService.GetApplicationForReviewAsync(id);

            if (application == null)
            {
                return NotFound();
            }

            var model = MapToApplicationReviewDetailsViewModel(application);

            return View(model);
        }




        [HttpGet]
        public async Task<IActionResult> ViewDocument(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var document = await _applicationService.GetDocumentFileAsync(id);

            if (document == null)
            {
                return NotFound();
            }

            return File(document.Content, document.ContentType);
        }




        // to approve an application
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id, string? employeeMessage)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var employeeId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(employeeId))
            {
                return Unauthorized();
            }

            var result = await _applicationService.ApproveApplicationAsync(
                    new ApproveApplicationRequest
                    {
                        ApplicationId = id,
                        EmployeeId = employeeId,
                        EmployeeMessage = employeeMessage
                    }
            );

            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = result.ErrorMessage ?? "The application could not be approved.";
                return RedirectToAction(nameof(Index), new { id });
            }

            TempData["SuccessMessage"] = $"Application approved successfully. Protocol number: {result.ProtocolNumber}";

            return RedirectToAction(nameof(Index), new { id });
        }



        // to reject an application
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string? employeeMessage)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var employeeId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(employeeId))
            {
                return Unauthorized();
            }

            var result = await _applicationService.RejectApplicationAsync(
                    new RejectApplicationRequest
                    {
                        ApplicationId = id,
                        EmployeeId = employeeId,
                        EmployeeMessage = employeeMessage
                    }
            );

            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = result.ErrorMessage ?? "The application could not be rejected.";

                return RedirectToAction(nameof(Index), new { id });
            }

            TempData["SuccessMessage"] = "Application rejected successfully.";

            return RedirectToAction(nameof(Index), new { id });
        }



        // for the timeline of each application
        [HttpGet]
        public async Task<IActionResult> Timeline(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var timeline = await _applicationService.GetEmployeeApplicationTimelineAsync(id);

            if (timeline == null)
            {
                return NotFound();
            }

            var model = new ApplicationTimelineViewModel
            {
                ApplicationId = timeline.ApplicationId,
                ApplicationTypeName = timeline.ApplicationTypeName,
                Status = timeline.Status,
                ProtocolNumber = timeline.ProtocolNumber,
                Items = timeline.Items
                    .Select(x => new ApplicationTimelineItemViewModel
                    {
                        Title = x.Title,
                        Description = x.Description,
                        OccurredAt = x.OccurredAt,
                        CssClass = x.CssClass
                    })
                    .ToList()
            };

            return View(model);
        }



        // to generate AI review for an application
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateAiReviewSummary(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var application = await _applicationService.GetApplicationForReviewAsync(id);

            if (application == null)
            {
                return NotFound();
            }

            var model = MapToApplicationReviewDetailsViewModel(application);

            var aiResult = await _applicationAiReviewService.GenerateForApplicationAsync(id);

            if (!aiResult.Succeeded)
            {
                TempData["ErrorMessage"] = aiResult.ErrorMessage ?? "The AI assistant is currently unavailable!";

                return View("Details", model);
            }

            model.AiReviewSummary = new AiReviewSummaryViewModel
            {
                Summary = aiResult.Summary,
                KeyInformation = aiResult.KeyInformation.ToList(),
                PdfReviewFocus = aiResult.PdfReviewFocus.ToList(),
                Disclaimer = aiResult.Disclaimer
            };

            return View("Details", model);
        }




        // util mapping method 
        private static ApplicationReviewDetailsViewModel MapToApplicationReviewDetailsViewModel(ApplicationReviewDetails application)
        {
            return new ApplicationReviewDetailsViewModel
            {
                Id = application.Id,
                ApplicationTypeName = application.ApplicationTypeName,

                CitizenFirstName = application.CitizenFirstName,
                CitizenLastName = application.CitizenLastName,
                CitizenEmail = application.CitizenEmail,

                SubmissionDate = application.SubmissionDate,

                Status = application.Status,

                Description = application.Description,

                Fields = application.Fields
                    .OrderBy(x => x.DisplayOrder)
                    .Select(x => new ApplicationReviewFieldViewModel
                    {
                        Label = x.Label,
                        Value = x.Value,
                        DisplayOrder = x.DisplayOrder
                    })
                    .ToList(),

                Documents = application.Documents
                    .Select(x => new ApplicationReviewDocumentViewModel
                    {
                        DocumentId = x.DocumentId,
                        RequirementName = x.RequirementName,
                        OriginalFileName = x.OriginalFileName,
                        UploadDate = x.UploadDate,
                        IsRequired = x.IsRequired
                    })
                    .ToList()
            };
        }
    }
}