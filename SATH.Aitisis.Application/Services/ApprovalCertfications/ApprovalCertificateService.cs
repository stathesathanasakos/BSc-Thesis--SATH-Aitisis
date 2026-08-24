using SATH.Aitisis.App.Common.Users;
using SATH.Aitisis.App.Services.Applications;
using SATH.Aitisis.Domain.Enums;

namespace SATH.Aitisis.App.Services.ApprovalCertificates
{
    public sealed class ApprovalCertificateService : IApprovalCertificateService
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IUserLookupService _userLookupService;
        private readonly IApprovalCertificatePdfGenerator _pdfGenerator;

        public ApprovalCertificateService(
            IApplicationRepository applicationRepository,
            IUserLookupService userLookupService,
            IApprovalCertificatePdfGenerator pdfGenerator)
        {
            _applicationRepository = applicationRepository;
            _userLookupService = userLookupService;
            _pdfGenerator = pdfGenerator;
        }

        public async Task<ApprovalCertificateFile?> GenerateForCitizenAsync(int applicationId,string citizenId)
        {
            if (applicationId <= 0 || string.IsNullOrWhiteSpace(citizenId))
            {
                return null;
            }

            var application = await _applicationRepository.GetApprovedApplicationForCertificateAsync(applicationId,citizenId);

            if (application == null)
            {
                return null;
            }

            if (application.Status != ApplicationStatus.Approved || string.IsNullOrWhiteSpace(application.ProtocolNumber) || !application.DecisionDate.HasValue || string.IsNullOrWhiteSpace(application.ReviewedByUserId))
            {
                return null;
            }

            var citizen = await _userLookupService.GetByIdAsync(application.CitizenId);

            var employee = await _userLookupService.GetByIdAsync(application.ReviewedByUserId);

            if (citizen == null || employee == null)
            {
                return null;
            }

            var data = new ApprovalCertificateData
            {
                ApplicationId = application.Id,
                ProtocolNumber = application.ProtocolNumber,
                ApplicationTypeName = application.ApplicationType.Name,
                SubmissionDate = application.SubmissionDate,
                ApprovalDate = application.DecisionDate.Value,
                CitizenFullName = BuildFullName(citizen),
                CitizenEmail = citizen.Email,
                EmployeeFullName = BuildFullName(employee),

                Fields = application.FieldValues
                    .OrderBy(x => x.ApplicationTypeField.DisplayOrder)
                    .Select(x => new ApprovalCertificateFieldItem
                    {
                        Label = x.ApplicationTypeField.Label,
                        Value = x.Value,
                        DisplayOrder =
                            x.ApplicationTypeField.DisplayOrder
                    })
                    .ToArray()
            };

            var content = _pdfGenerator.Generate(data);

            if (content.Length == 0)
            {
                return null;
            }

            return new ApprovalCertificateFile
            {
                Content = content,
                ContentType = "application/pdf",
                FileName = $"Approval-Certificate-{application.ProtocolNumber}.pdf"
            };
        }

        private static string BuildFullName(UserProfileInfo user)
        {
            var fullName = $"{user.FirstName} {user.LastName}".Trim();

            if (!string.IsNullOrWhiteSpace(fullName))
            {
                return fullName;
            }

            return user.Email;
        }
    }
} 
