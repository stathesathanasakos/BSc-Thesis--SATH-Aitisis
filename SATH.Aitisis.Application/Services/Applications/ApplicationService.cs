using SATH.Aitisis.Domain.Entities;
using SATH.Aitisis.Domain.Enums;
using DomainApplication = SATH.Aitisis.Domain.Entities.Application;
using DomainDocument = SATH.Aitisis.Domain.Entities.Document;
using SATH.Aitisis.App.Common.Users;
using SATH.Aitisis.App.Services.Notifications;
using SATH.Aitisis.App.Services.Applications.StatusTransitions;



namespace SATH.Aitisis.App.Services.Applications
{
    public sealed class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _repository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IUserLookupService _userLookupService;
        private readonly INotificationService _notificationService;
        private readonly IApplicationStatusTransitionPolicy _transitionPolicy;


        public ApplicationService(IApplicationRepository repository, IFileStorageService fileStorageService, IUserLookupService userLookupService, INotificationService notificationService, IApplicationStatusTransitionPolicy transitionPolicy)
        {
            _repository           = repository;
            _fileStorageService   = fileStorageService;
            _userLookupService    = userLookupService;
            _notificationService  = notificationService;
            _transitionPolicy     = transitionPolicy;
        }

        public async Task<IReadOnlyCollection<AvailableApplicationTypeItem>> GetAvailableApplicationTypesAsync()
        {
            var applicationTypes = await _repository.GetActiveApplicationTypesAsync();

            return applicationTypes
                .OrderBy(x => x.Name)
                .Select(x => new AvailableApplicationTypeItem
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    RequiresPayment = x.RequiresPayment,
                    FeeAmount = x.FeeAmount
                })
                .ToArray();
        }

        public async Task<ApplicationSubmissionForm?>GetSubmissionFormAsync(int applicationTypeId)
        {
            var applicationType = await _repository.GetApplicationTypeWithConfigurationAsync(applicationTypeId);

            if (applicationType == null ||
                !applicationType.IsActive)
            {
                return null;
            }

            return new ApplicationSubmissionForm
            {
                ApplicationTypeId = applicationType.Id,
                ApplicationTypeName = applicationType.Name,
                Description = applicationType.Description,
                RequiresPayment = applicationType.RequiresPayment,
                FeeAmount = applicationType.FeeAmount,

                Fields = applicationType.Fields
                    .OrderBy(x => x.DisplayOrder)
                    .Select(x => new ApplicationFormFieldItem
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Label = x.Label,
                        FieldType = x.FieldType,
                        IsRequired = x.IsRequired,
                        DisplayOrder = x.DisplayOrder
                    })
                    .ToArray(),

                DocumentRequirements =
                    applicationType.DocumentRequirements
                        .OrderBy(x => x.DisplayOrder)
                        .Select(x => new ApplicationDocumentRequirementItem
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Description = x.Description,
                            IsRequired = x.IsRequired,
                            DisplayOrder = x.DisplayOrder
                        })
                        .ToArray()
            };
        }

        public async Task<SubmitApplicationResult> SubmitApplicationAsync(SubmitApplicationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.CitizenId))
            {
                return Failure("Citizen information is required.");
            }

            var applicationType = await _repository.GetApplicationTypeWithConfigurationAsync(request.ApplicationTypeId);

            if (applicationType == null ||
                !applicationType.IsActive)
            {
                return Failure("The selected application type is not available.");
            }

            var submittedValues =
                request.FieldValues.ToDictionary(
                    x => x.ApplicationTypeFieldId,
                    x => x.Value);

            foreach (var field in applicationType.Fields)
            {
                if (field.IsRequired)
                {
                    if (!submittedValues.TryGetValue(
                            field.Id,
                            out var value) ||
                        string.IsNullOrWhiteSpace(value))
                    {
                        return Failure($"The field '{field.Label}' is required.");
                    }
                }
            }

            var validFieldIds = applicationType.Fields
                .Select(x => x.Id)
                .ToHashSet();

            if (request.FieldValues.Any(
                x => !validFieldIds.Contains(
                    x.ApplicationTypeFieldId)))
            {
                return Failure("One or more submitted fields are invalid.");
            }

            var application = new DomainApplication
            {
                CitizenId = request.CitizenId,
                ApplicationTypeId = applicationType.Id,
                Description =
                    string.IsNullOrWhiteSpace(request.Description)
                        ? null
                        : request.Description.Trim(),

                Status = ApplicationStatus.Submitted,
                SubmissionDate = DateTime.UtcNow
            };

            await _repository.AddApplicationAsync(application);

            await _repository.SaveChangesAsync();

            var fieldValues = request.FieldValues
                .Where(x => !string.IsNullOrWhiteSpace(x.Value))
                .Select(x => new ApplicationFieldValue
                {
                    ApplicationId = application.Id,
                    ApplicationTypeFieldId =
                        x.ApplicationTypeFieldId,
                    Value = x.Value.Trim()
                })
                .ToArray();

            if (fieldValues.Length > 0)
            {
                await _repository.AddFieldValuesAsync(fieldValues);
                await _repository.SaveChangesAsync();
            }

            return new SubmitApplicationResult
            {
                Succeeded = true,
                ApplicationId = application.Id
            };
        }

        private static SubmitApplicationResult Failure(string error)
        {
            return new SubmitApplicationResult
            {
                Succeeded = false,
                Errors = new[] { error }
            };
        }




        public async Task<UploadApplicationDocumentResult>UploadDocumentAsync(UploadApplicationDocumentRequest request)
        {
            if (request.ApplicationId <= 0 ||
                string.IsNullOrWhiteSpace(request.CitizenId))
            {
                return new UploadApplicationDocumentResult
                {
                    Succeeded = false,
                    Errors = new[]
                    {
                "Invalid application information."
            }
                };
            }

            var application = await _repository.GetApplicationForDocumentUploadAsync(request.ApplicationId, request.CitizenId);

            if (application == null)
            {
                return new UploadApplicationDocumentResult
                {
                    Succeeded = false,
                    Errors = new[]
                    {
                "Application was not found."
            }
                };
            }

            // Only PDF documents are accepted.
            var extension = Path.GetExtension(request.OriginalFileName);

            if (!string.Equals(extension, ".pdf", StringComparison.OrdinalIgnoreCase) || !string.Equals( request.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase))
            {
                return new UploadApplicationDocumentResult
                {
                    Succeeded = false,
                    Errors = new[]
                    { "Only PDF files are allowed."}
                };
            }

            const long maxFileSize = 10 * 1024 * 1024; // 10 MB

            if (request.FileSize <= 0 || request.FileSize > maxFileSize)
            {
                return new UploadApplicationDocumentResult
                {
                    Succeeded = false,
                    Errors = new[]
                    {
                "The PDF file must not exceed 10 MB."
            }
                };
            }

            if (request.DocumentRequirementId.HasValue)
            {
                var validRequirement =
                    application.ApplicationType
                        .DocumentRequirements
                        .Any(x =>
                            x.Id ==
                            request.DocumentRequirementId.Value);

                if (!validRequirement)
                {
                    return new UploadApplicationDocumentResult
                    {
                        Succeeded = false,
                        Errors = new[]
                        {"The selected document requirement is invalid."}
                    };
                }
            }

            var storedFileName = await _fileStorageService.SaveAsync(request.Content, request.OriginalFileName);

            var document = new Document
            {
                ApplicationId = application.Id,
                DocumentRequirementId = request.DocumentRequirementId,

                UploadedByUserId = request.CitizenId,

                OriginalFileName = Path.GetFileName(request.OriginalFileName),

                StoredFileName = storedFileName,

                ContentType = request.ContentType,

                FileSize = request.FileSize,

                UploadDate = DateTime.UtcNow
            };

            await _repository.AddDocumentAsync(document);
            await _repository.SaveChangesAsync();

            return new UploadApplicationDocumentResult
            {
                Succeeded = true
            };
        }


        public async Task<ApplicationDocumentsInfo?>GetApplicationDocumentsInfoAsync(int applicationId, string citizenId)
        {
            var application = await _repository.GetApplicationForDocumentUploadAsync(applicationId,citizenId);

            if (application == null)
            {
                return null;
            }

            var uploadedDocuments = await _repository.GetApplicationDocumentsAsync(applicationId);

            var uploadedRequirementIds =
                uploadedDocuments
                    .Where(x => x.DocumentRequirementId.HasValue)
                    .Select(x => x.DocumentRequirementId!.Value)
                    .ToHashSet();

            var documents =
                application.ApplicationType.DocumentRequirements
                    .OrderBy(x => x.DisplayOrder)
                    .Select(x => new ApplicationDocumentStatusItem
                    {
                        DocumentRequirementId = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        IsRequired = x.IsRequired,
                        DisplayOrder = x.DisplayOrder,
                        IsUploaded = uploadedRequirementIds.Contains(x.Id)
                    })
                    .ToArray();

            return new ApplicationDocumentsInfo
            {
                ApplicationId = application.Id,
                ApplicationTypeId = application.ApplicationTypeId,
                ApplicationTypeName = application.ApplicationType.Name,
                RequiresPayment = application.ApplicationType.RequiresPayment,
                Documents = documents
            };
        }


        public async Task<IReadOnlyCollection<ApplicationHistoryItem>> GetCitizenApplicationsAsync(string citizenId)
        {
            if (string.IsNullOrWhiteSpace(citizenId))
            {
                return Array.Empty<ApplicationHistoryItem>();
            }

            var applications = await _repository.GetByCitizenIdAsync(citizenId);

            return applications
                .Select(application =>
                {
                    var requiresPayment = application.ApplicationType.RequiresPayment;

                    var paymentStatus = application.Payment?.Status;

                    var canPay = requiresPayment && application.ApplicationType.FeeAmount.HasValue && application.ApplicationType.FeeAmount.Value > 0 && paymentStatus != PaymentStatus.Completed;

                    return new ApplicationHistoryItem
                        {
                            Id = application.Id,
                            ApplicationTypeName = application.ApplicationType.Name,
                            Status = application.Status,
                            SubmissionDate = application.SubmissionDate, 
                            ProtocolNumber = application.ProtocolNumber,
                            DecisionDate = application.DecisionDate,
                            RequiresPayment = requiresPayment,
                            PaymentStatus = paymentStatus,
                            CanPay = canPay
                        };
                 })
                .ToArray();
        }







        public async Task<IReadOnlyCollection<PendingApplicationItem>>GetPendingApplicationsAsync()
        {
            var applications = await _repository.GetPendingApplicationsAsync();

            return applications
                .Select(x => new PendingApplicationItem
                    {
                        Id = x.Id,
                        ApplicationTypeName = x.ApplicationType.Name,
                        SubmissionDate = x.SubmissionDate
                    })
                .ToArray();
        }


        public async Task<ApplicationReviewDetails?> GetApplicationForReviewAsync(int applicationId)
        {
            var application = await _repository.GetApplicationForReviewAsync(applicationId);

            if (application == null)
            {
                return null;
            }

            var citizen = await _userLookupService.GetByIdAsync(application.CitizenId);

            return new ApplicationReviewDetails
            {
                Id = application.Id,
                ApplicationTypeName = application.ApplicationType.Name,

                CitizenId = application.CitizenId,

                CitizenFirstName = citizen?.FirstName ?? string.Empty,
                CitizenLastName = citizen?.LastName ?? string.Empty,
                CitizenEmail = citizen?.Email ?? string.Empty,

                SubmissionDate = application.SubmissionDate,

                Status = application.Status.ToString(),

                Description = application.Description,

                Fields = application.FieldValues
                    .OrderBy(x => x.ApplicationTypeField.DisplayOrder)
                    .Select(x => new ApplicationReviewFieldItem
                    {
                        FieldId = x.ApplicationTypeFieldId,
                        Label = x.ApplicationTypeField.Label,
                        Value = x.Value,
                        DisplayOrder = x.ApplicationTypeField.DisplayOrder
                    })
                    .ToArray(),

                Documents = application.Documents
                    .OrderBy(x => x.DocumentRequirement != null
                        ? x.DocumentRequirement.DisplayOrder
                        : int.MaxValue)
                    .Select(x => new ApplicationReviewDocumentItem
                    {
                        DocumentId = x.Id,

                        RequirementName = x.DocumentRequirement != null
                            ? x.DocumentRequirement.Name
                            : "Other Document",

                        OriginalFileName = x.OriginalFileName,

                        UploadDate = x.UploadDate,

                        IsRequired = x.DocumentRequirement?.IsRequired ?? false
                    })
                    .ToArray()
            };
        }



        
        public async Task<ApplicationDocumentFile?> GetDocumentFileAsync(int documentId)
        {
            var document = await _repository.GetDocumentByIdAsync(documentId);

            if (document == null)
            {
                return null;
            }

            var content = await _fileStorageService.OpenReadAsync(document.StoredFileName);

            if (content == null)
            {
                return null;
            }

            return new ApplicationDocumentFile
            {
                Content = content,
                ContentType = document.ContentType,
                FileName = document.OriginalFileName
            };
        }







        public async Task<ApplicationDecisionResult> ApproveApplicationAsync(ApproveApplicationRequest request)
        {
            if (request.ApplicationId <= 0)
            {
                return ApplicationDecisionResult.Failure("Invalid application.");
            }

            if (string.IsNullOrWhiteSpace(request.EmployeeId))
            {
                return ApplicationDecisionResult.Failure("Employee information is required.");
            }

            var application = await _repository.GetByIdForUpdateAsync( request.ApplicationId);

            if (application == null)
            {
                return ApplicationDecisionResult.Failure("Application was not found.");
            }

            if (!_transitionPolicy.CanApprove(application.Status))
            {
                return ApplicationDecisionResult.Failure("Only pending applications can be approved.");
            }

            var decisionDate = DateTime.UtcNow;

            var protocolNumber = GenerateProtocolNumber(application.Id,decisionDate);

            application.Status = ApplicationStatus.Approved;

            application.ReviewedByUserId = request.EmployeeId;

            application.DecisionDate = decisionDate;

            application.ProtocolNumber = protocolNumber;

            await _repository.SaveChangesAsync();

            await _notificationService.CreateAsync(new CreateNotificationRequest
                {
                    UserId = application.CitizenId,
                    ApplicationId = application.Id,
                    Title = "Application approved",
                    Message = BuildApprovedNotificationMessage(application.ApplicationType.Name, protocolNumber, request.EmployeeMessage),
                    Type = NotificationType.Info
                }
            );

            return ApplicationDecisionResult.Success(protocolNumber);
        }


        public async Task<ApplicationDecisionResult> RejectApplicationAsync(RejectApplicationRequest request)
        {
            if (request.ApplicationId <= 0)
            {
                return ApplicationDecisionResult.Failure("Invalid application.");
            }

            if (string.IsNullOrWhiteSpace(request.EmployeeId))
            {
                return ApplicationDecisionResult.Failure("Employee information is required.");
            }

            var application = await _repository.GetByIdForUpdateAsync(request.ApplicationId);

            if (application == null)
            {
                return ApplicationDecisionResult.Failure("Application was not found.");
            }

            if (!_transitionPolicy.CanReject(application.Status))
            {
                return ApplicationDecisionResult.Failure("Only pending applications can be rejected.");
            }

            application.Status = ApplicationStatus.Rejected;

            application.ReviewedByUserId = request.EmployeeId;

            application.DecisionDate = DateTime.UtcNow;

            application.ProtocolNumber = null;

            await _repository.SaveChangesAsync();

            await _notificationService.CreateAsync(new CreateNotificationRequest
                {
                    UserId = application.CitizenId,
                    ApplicationId = application.Id,
                    Title = "Application rejected",
                    Message = BuildRejectedNotificationMessage(application.ApplicationType.Name,request.EmployeeMessage),
                    Type = NotificationType.Info
                }
            );

            return ApplicationDecisionResult.Success();
        }


        // and this is a private method to generate a protocol number based on the application ID and decision date
        private static string GenerateProtocolNumber(int applicationId,DateTime decisionDate)
        {
            return $"SATH-{decisionDate.Year}-{applicationId:D6}";
        }



        // to build the notification message for approved applications
        private static string BuildApprovedNotificationMessage(string applicationTypeName, string protocolNumber, string? employeeMessage)
        {
            var defaultMessage = $"Your application for {applicationTypeName} has been approved. " + $"Protocol number: {protocolNumber}.";

            if (string.IsNullOrWhiteSpace(employeeMessage))
            {
                return defaultMessage;
            }

            return defaultMessage +
                Environment.NewLine +
                Environment.NewLine +
                "Employee message: " +
                employeeMessage.Trim();
        }

        // quickly build the notification message for rejected applications
        private static string BuildRejectedNotificationMessage(string applicationTypeName, string? employeeMessage)
        {
            var defaultMessage = $"Your application for {applicationTypeName} has been rejected.";

            if (string.IsNullOrWhiteSpace(employeeMessage))
            {
                return defaultMessage;
            }

            return defaultMessage +
                Environment.NewLine +
                Environment.NewLine +
                "Employee message: " +
                employeeMessage.Trim();
        }




        // to get the application timeline for a citizen
        public async Task<ApplicationTimelineDetails?> GetCitizenApplicationTimelineAsync(int applicationId,string citizenId)
        {
            if (applicationId <= 0 ||
                string.IsNullOrWhiteSpace(citizenId))
            {
                return null;
            }

            var application = await _repository.GetTimelineForCitizenAsync(applicationId, citizenId);

            if (application == null)
            {
                return null;
            }

            return BuildTimeline(application);
        }

        
        // to get the application timeline for an employee
        public async Task<ApplicationTimelineDetails?>GetEmployeeApplicationTimelineAsync(int applicationId)
        {
            if (applicationId <= 0)
            {
                return null;
            }

            var application = await _repository.GetTimelineForEmployeeAsync(applicationId);

            if (application == null)
            {
                return null;
            }

            return BuildTimeline(application);
        }


        // to build the application timeline details based on the application entity
        private static ApplicationTimelineDetails BuildTimeline(DomainApplication application)
        {
            var items = new List<ApplicationTimelineItem>();

            items.Add(
                new ApplicationTimelineItem
                {
                    Title = "Application submitted",
                    Description =
                        $"The citizen submitted the application for {application.ApplicationType.Name}.",
                    OccurredAt = application.SubmissionDate,
                    CssClass = "primary"
                });

            foreach (var document in application.Documents.OrderBy(x => x.UploadDate))
            {
                var requirementName =
                    document.DocumentRequirement != null
                        ? document.DocumentRequirement.Name
                        : "Other document";

                items.Add(
                    new ApplicationTimelineItem
                    {
                        Title = "Document uploaded",
                        Description = $"{document.OriginalFileName} uploaded for requirement: {requirementName}.",
                        OccurredAt = document.UploadDate,
                        CssClass = "secondary"
                    });
            }

            if (application.DecisionDate.HasValue)
            {
                if (application.Status == ApplicationStatus.Approved)
                {
                    items.Add(
                        new ApplicationTimelineItem
                        {
                            Title = "Application approved",
                            Description =
                                string.IsNullOrWhiteSpace(application.ProtocolNumber)
                                    ? "The application was approved."
                                    : $"The application was approved. Protocol number: {application.ProtocolNumber}.",
                            OccurredAt = application.DecisionDate.Value,
                            CssClass = "success"
                        });
                }
                else if (application.Status == ApplicationStatus.Rejected)
                {
                    items.Add(
                        new ApplicationTimelineItem
                        {
                            Title = "Application rejected",
                            Description = "The application was rejected by an employee.",
                            OccurredAt = application.DecisionDate.Value,
                            CssClass = "danger"
                        });
                }
            }

            return new ApplicationTimelineDetails
            {
                ApplicationId = application.Id,
                ApplicationTypeName = application.ApplicationType.Name,
                Status = application.Status,
                ProtocolNumber = application.ProtocolNumber,
                Items = items.OrderBy(x => x.OccurredAt).ToArray()
            };
        }
    }
}