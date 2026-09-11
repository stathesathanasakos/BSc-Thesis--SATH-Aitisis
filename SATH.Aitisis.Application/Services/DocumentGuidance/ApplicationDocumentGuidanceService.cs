using SATH.Aitisis.App.Services.Applications;

namespace SATH.Aitisis.App.Services.DocumentGuidance
{
    public sealed class ApplicationDocumentGuidanceService : IApplicationDocumentGuidanceService
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly ICitizenDocumentGuidanceAssistant _guidanceAssistant;

        public ApplicationDocumentGuidanceService(IApplicationRepository applicationRepository, ICitizenDocumentGuidanceAssistant guidanceAssistant)
        {
            _applicationRepository = applicationRepository;
            _guidanceAssistant = guidanceAssistant;
        }


        public async Task<DocumentGuidanceResult> GenerateForApplicationAsync(int applicationId, string citizenId)
        {
            if (applicationId <= 0 || string.IsNullOrWhiteSpace(citizenId))
            {
                return DocumentGuidanceResult.Failure("Invalid application information.");
            }

            var application = await _applicationRepository.GetApplicationForSubmissionCompletionAsync(applicationId, citizenId);

            if (application == null)
            {
                return DocumentGuidanceResult.Failure("Application was not found.");
            }

            if (application.ApplicationType == null)
            {
                return DocumentGuidanceResult.Failure("Application type could not be loaded.");
            }

            var documents = application.ApplicationType.DocumentRequirements
                .OrderBy(x => x.DisplayOrder)
                .Select(x => new DocumentGuidanceDocumentItem
                {
                    Name = x.Name,
                    Description = x.Description,
                    IsRequired = x.IsRequired,
                    DisplayOrder = x.DisplayOrder
                })
                .ToArray();

            if (documents.Length == 0)
            {
                return DocumentGuidanceResult.Failure("This application type does not require supporting documents.");
            }

            var request = new DocumentGuidanceRequest
            {
                ApplicationId = application.Id,
                ApplicationTypeName = application.ApplicationType.Name,
                Documents = documents
            };

            return await _guidanceAssistant.GenerateGuidanceAsync(request);
        }

    }
}