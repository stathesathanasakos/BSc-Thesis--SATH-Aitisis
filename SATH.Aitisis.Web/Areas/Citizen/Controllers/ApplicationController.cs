using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SATH.Aitisis.App.Common.Constants;
using SATH.Aitisis.App.Services.Applications;
using SATH.Aitisis.Domain.Entities;
using SATH.Aitisis.Domain.Enums;
using SATH.Aitisis.Web.Areas.Citizen.ViewModels.Applications;
using System.Security.Claims;
using SATH.Aitisis.Web.ViewModels.Applications;
using SATH.Aitisis.App.Services.ApprovalCertificates;
using SATH.Aitisis.App.Services.Payments;
using SATH.Aitisis.App.Services.DocumentGuidance;



namespace SATH.Aitisis.Web.Areas.Citizen.Controllers
{
    [Area("Citizen")]
    [Authorize(Roles = AppRoles.Citizen)]
    public sealed class ApplicationsController : Controller
    {
        private readonly IApplicationService _applicationService;
        private readonly IApprovalCertificateService _approvalCertificateService;
        private readonly IPaymentService _paymentService;
        private readonly IApplicationDocumentGuidanceService _documentGuidanceService;

        public ApplicationsController(IApplicationService applicationService, IApprovalCertificateService approvalCertificateService, IPaymentService paymentService, IApplicationDocumentGuidanceService documentGuidanceService)
        {
            _applicationService = applicationService;
            _approvalCertificateService = approvalCertificateService;
            _paymentService = paymentService;
            _documentGuidanceService = documentGuidanceService;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var applicationTypes = await _applicationService.GetAvailableApplicationTypesAsync();

            var model = applicationTypes
                .Select(x => new AvailableApplicationTypeViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    RequiresPayment = x.RequiresPayment,
                    FeeAmount = x.FeeAmount
                })
                .ToList();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int id)
        {
            var form = await _applicationService.GetSubmissionFormAsync(id);

            if (form == null)
            {
                return NotFound();
            }

            var model = new SubmitApplicationViewModel
            {
                ApplicationTypeId = form.ApplicationTypeId,
                ApplicationTypeName = form.ApplicationTypeName,
                ApplicationTypeDescription = form.Description,
                RequiresPayment = form.RequiresPayment,
                FeeAmount = form.FeeAmount,

                Fields = form.Fields
                    .Select(x => new ApplicationFieldInputViewModel
                    {
                        FieldId = x.Id,
                        Name = x.Name,
                        Label = x.Label,
                        FieldType = x.FieldType,
                        IsRequired = x.IsRequired,
                        DisplayOrder = x.DisplayOrder
                    })
                    .ToList(),

                DocumentRequirements = form.DocumentRequirements
                        .Select(x =>
                            new ApplicationDocumentRequirementViewModel
                            {
                                Id = x.Id,
                                Name = x.Name,
                                Description = x.Description,
                                IsRequired = x.IsRequired,
                                DisplayOrder = x.DisplayOrder
                            })
                        .ToList()
            };

            return View(model);
        }



        // to show the documents upload page for the application
        [HttpGet]
        public async Task<IActionResult> Documents(int applicationId)
        {
            var citizenId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var info = await _applicationService.GetApplicationDocumentsInfoAsync(applicationId, citizenId);

            if (string.IsNullOrWhiteSpace(citizenId))
            {
                return Unauthorized();
            }

            if (info == null)
            {
                return NotFound();
            }

            var model = MapDocumentsViewModel(info);

            return View(model);
        }


        // to generate AI document guidance for the application
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateDocumentGuidance(int applicationId)
        {
            var citizenId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(citizenId))
            {
                return Unauthorized();
            }

            var info = await _applicationService.GetApplicationDocumentsInfoAsync(applicationId, citizenId);

            if (info == null)
            {
                return NotFound();
            }

            var model = MapDocumentsViewModel(info);
            var result = await _documentGuidanceService.GenerateForApplicationAsync(applicationId, citizenId);

            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = result.ErrorMessage ?? "AI document guidance could not be generated.";
                return View("Documents", model);
            }

            model.AiDocumentGuidance = new AiDocumentGuidanceViewModel
            {
                Summary = result.Summary,
                GeneralAdvice = result.GeneralAdvice,
                Disclaimer = result.Disclaimer,
                Items = result.Items
                    .Select(x => new AiDocumentGuidanceItemViewModel
                    {
                        DocumentName = x.DocumentName,
                        Steps = x.Steps.ToList()
                    })
                    .ToList()
            };

            return View("Documents", model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubmitApplicationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var citizenId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(citizenId))
            {
                return Unauthorized();
            }

            var request = new SubmitApplicationRequest
            {
                CitizenId = citizenId,
                ApplicationTypeId = model.ApplicationTypeId,
                Description = model.Description,

                FieldValues = model.Fields
                    .Select(x => new SubmittedFieldValue
                    {
                        ApplicationTypeFieldId = x.FieldId,
                        Value = x.Value ?? string.Empty
                    })
                    .ToArray()
            };

            var result =await _applicationService.SubmitApplicationAsync(request);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return View(model);
            }

            TempData["SuccessMessage"] = "Application information saved. Please upload the required documents.";

            //return RedirectToAction(nameof(Index));
            /*
            return RedirectToAction(
    nameof(Documents),
    new
    {
        applicationId = result.ApplicationId!.Value,
        applicationTypeId = model.ApplicationTypeId
    });
            */

            return RedirectToAction(
                    nameof(Documents),
                        new
                        {
                            applicationId = result.ApplicationId!.Value
                        });
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadDocument(UploadDocumentViewModel model)
        {
            if (!ModelState.IsValid || model.File == null)
            {
                TempData["ErrorMessage"] = "Please select a PDF file.";

                return RedirectToAction(
                    nameof(Documents),
                    new
                    {
                        applicationId = model.ApplicationId,
                        applicationTypeId = model.ApplicationTypeId
                    });
            }

            var citizenId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(citizenId))
            {
                return Unauthorized();
            }

            await using var stream = model.File.OpenReadStream();

            var request = new UploadApplicationDocumentRequest
            {
                ApplicationId = model.ApplicationId,
                CitizenId = citizenId,

                DocumentRequirementId = model.DocumentRequirementId,

                OriginalFileName = model.File.FileName,

                ContentType = model.File.ContentType,

                FileSize = model.File.Length,

                Content = stream
            };

            var result = await _applicationService.UploadDocumentAsync(request);

            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = string.Join(" ", result.Errors);
            }
            else
            {
                TempData["SuccessMessage"] ="Document uploaded successfully.";
            }

            return RedirectToAction(
                nameof(Documents),
                    new
                    {
                        applicationId = model.ApplicationId
                    });
        }


        /*
        [HttpGet]
        public async Task<IActionResult> ContinueSubmission(int applicationId)
        {
            var citizenId =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(citizenId))
            {
                return Unauthorized();
            }

            var info = await _applicationService.GetApplicationDocumentsInfoAsync(applicationId, citizenId);

            if (info == null)
            {
                return NotFound();
            }

            if (!info.RequiredDocumentsCompleted)
            {
                TempData["ErrorMessage"] = "All required documents must be uploaded before continuing.";

                return RedirectToAction(nameof(Documents),new { applicationId });
            }

            if (info.RequiresPayment)
            {
                return RedirectToAction(nameof(Payment), new { applicationId });
            }

            return RedirectToAction(nameof(Confirmation), new { applicationId });
        }
        */

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContinueSubmission(int applicationId)
        {
            var citizenId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(citizenId))
            {
                return Unauthorized();
            }

            var documentsInfo = await _applicationService.GetApplicationDocumentsInfoAsync(applicationId, citizenId);

            if (documentsInfo == null)
            {
                return NotFound();
            }

            if (!documentsInfo.RequiredDocumentsCompleted)
            {
                TempData["ErrorMessage"] = "Please upload all required documents before continuing.";
                return RedirectToAction(nameof(Documents), new { applicationId });
            }

            var paymentDetails = await _paymentService.GetPaymentDetailsAsync(applicationId, citizenId);

            if (paymentDetails == null)
            {
                return NotFound();
            }

            if (paymentDetails.RequiresPayment)
            {
                if (paymentDetails.CanStartPayment)
                {
                    return RedirectToAction(
                        "Details",
                        "Payments",
                        new
                        {
                            area = "Citizen",
                            applicationId
                        });
                }

                TempData["ErrorMessag"] = "Payment must be completed before the application can be submitted.";

                return RedirectToAction(nameof(Documents), new { applicationId });
            }

            var completionResult =
                await _applicationService.CompleteSubmissionAsync(
                    new CompleteApplicationSubmissionRequest
                    {
                        ApplicationId = applicationId,
                        CitizenId = citizenId
                    });

            if (!completionResult.Succeeded)
            {
                TempData["ErrorMessage"] = completionResult.ErrorMessage ?? "The application could not be submitted.";
                return RedirectToAction(nameof(Documents), new { applicationId });
            }

            return RedirectToAction(nameof(Confirmation), new { applicationId });
        }




        [HttpGet]
        public async Task<IActionResult> Confirmation(int applicationId)
        {
            var citizenId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(citizenId))
            {
                return Unauthorized();
            }

            var info = await _applicationService.GetApplicationDocumentsInfoAsync(applicationId, citizenId);

            if (info == null)
            {
                return NotFound();
            }

            if (info.Status != ApplicationStatus.Submitted || !info.RequiredDocumentsCompleted || info.RequiresPayment)
            {
                return RedirectToAction(nameof(Documents), new { applicationId });
            }

            ViewBag.ApplicationId = applicationId;
            ViewBag.ApplicationTypeName = info.ApplicationTypeName;

            return View();
        }



        /*
        [HttpGet]
        public IActionResult Payment(int applicationId)
        {
            return View();
        }
        */

        [HttpGet]
        public IActionResult Payment(int applicationId)
        {
            return RedirectToAction(
                "Details",
                "Payments",
                new
                {
                    area = "Citizen",
                    applicationId
                });
        }



        [HttpGet]
        public async Task<IActionResult> MyApplications()
        {
            var citizenId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(citizenId))
            {
                return Unauthorized();
            }

            var applications = await _applicationService.GetCitizenApplicationsAsync(citizenId);

            var model = applications
                .Select(x => new MyApplicationListItemViewModel
                {
                    Id = x.Id,
                    ApplicationTypeName = x.ApplicationTypeName,

                    StatusText = x.Status switch
                    {
                        ApplicationStatus.Submitted => "Submitted",
                        ApplicationStatus.Approved => "Approved",
                        ApplicationStatus.Rejected => "Rejected",
                        _ => x.Status.ToString()
                    },

                    StatusCssClass = x.Status switch
                    {
                        ApplicationStatus.Submitted => "bg-warning text-dark",
                        ApplicationStatus.Approved => "bg-success",
                        ApplicationStatus.Rejected => "bg-danger",
                        _ => "bg-secondary"
                    },

                    SubmissionDate = x.SubmissionDate,
                    ProtocolNumber = x.ProtocolNumber,
                    DecisionDate = x.DecisionDate,

                    RequiresPayment = x.RequiresPayment,
                    PaymentStatus = x.PaymentStatus,
                    CanPay = x.CanPay
                })
                .ToList();

            return View(model);
        }




        // for the timeline of the application
        [HttpGet]
        public async Task<IActionResult> Timeline(int id)
        {
            var citizenId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(citizenId))
            {
                return Unauthorized();
            }

            var timeline = await _applicationService.GetCitizenApplicationTimelineAsync(id, citizenId);

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


        // to download the approval certificate for the approved application
        [HttpGet]
        public async Task<IActionResult> DownloadCertificate(int id)
        {
            var citizenId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(citizenId))
            {
                return Unauthorized();
            }

            var certificate = await _approvalCertificateService.GenerateForCitizenAsync(id, citizenId);

            if (certificate == null)
            {
                return NotFound();
            }

            return File(certificate.Content, certificate.ContentType, certificate.FileName);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelDraft(int applicationId)
        {
            var citizenId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(citizenId))
            {
                return Unauthorized();
            }

            await _applicationService.DeleteDraftApplicationAsync(applicationId, citizenId);
            TempData["SuccessMessage"] = "Application draft was cancelled.";

            return RedirectToAction(nameof(Index));
        }




        // this is a helper method to map the ApplicationDocumentsInfo to ApplicationDocumentsViewModel
        private static ApplicationDocumentsViewModel MapDocumentsViewModel(ApplicationDocumentsInfo info)
        {
            return new ApplicationDocumentsViewModel
            {
                ApplicationId = info.ApplicationId,
                ApplicationTypeId = info.ApplicationTypeId,
                ApplicationTypeName = info.ApplicationTypeName,
                RequiresPayment = info.RequiresPayment,
                RequiredDocumentsCompleted = info.RequiredDocumentsCompleted,

                DocumentRequirements = info.Documents
                    .Select(x => new DocumentRequirementUploadItemViewModel
                    {
                        Id = x.DocumentRequirementId,
                        Name = x.Name,
                        Description = x.Description,
                        IsRequired = x.IsRequired,
                        DisplayOrder = x.DisplayOrder,
                        IsUploaded = x.IsUploaded
                    })
                    .ToList()
            };
        }

    }
}