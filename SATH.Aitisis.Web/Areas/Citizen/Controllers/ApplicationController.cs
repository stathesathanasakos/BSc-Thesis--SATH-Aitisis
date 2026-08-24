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



namespace SATH.Aitisis.Web.Areas.Citizen.Controllers
{
    [Area("Citizen")]
    [Authorize(Roles = AppRoles.Citizen)]
    public sealed class ApplicationsController : Controller
    {
        private readonly IApplicationService _applicationService;
        private readonly IApprovalCertificateService _approvalCertificateService;

        public ApplicationsController(IApplicationService applicationService, IApprovalCertificateService approvalCertificateService)
        {
            _applicationService = applicationService;
            _approvalCertificateService = approvalCertificateService;
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



        [HttpGet]
        public async Task<IActionResult> Documents(int applicationId)
        {
            var citizenId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(citizenId))
            {
                return Unauthorized();
            }

            var info = await _applicationService.GetApplicationDocumentsInfoAsync(applicationId,citizenId);

            if (info == null)
            {
                return NotFound();
            }

            var model = new ApplicationDocumentsViewModel
            {
                ApplicationId = info.ApplicationId,
                ApplicationTypeId = info.ApplicationTypeId,
                ApplicationTypeName = info.ApplicationTypeName,
                RequiresPayment = info.RequiresPayment,
                RequiredDocumentsCompleted =
                    info.RequiredDocumentsCompleted,

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

            return View(model);
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

            TempData["SuccessMessage"] = "Your application was submitted successfully.";

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


        [HttpGet]
        public async Task<IActionResult> Confirmation(int applicationId)
        {
            var citizenId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(citizenId))
            {
                return Unauthorized();
            }

            var info = await _applicationService.GetApplicationDocumentsInfoAsync(applicationId,citizenId);

            if (info == null)
            {
                return NotFound();
            }

            if (!info.RequiredDocumentsCompleted || info.RequiresPayment)
            {
                return RedirectToAction(nameof(Documents),new { applicationId });
            }

            ViewBag.ApplicationId = applicationId;
            ViewBag.ApplicationTypeName = info.ApplicationTypeName;

            return View();
        }




        [HttpGet]
        public IActionResult Payment(int applicationId)
        {
            return View();
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
                        ApplicationStatus.Submitted => "Pending",
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
                    DecisionDate = x.DecisionDate
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

            var timeline =
                await _applicationService.GetCitizenApplicationTimelineAsync(id, citizenId);

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
    }
}