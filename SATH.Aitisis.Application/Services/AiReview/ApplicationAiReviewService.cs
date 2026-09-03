using SATH.Aitisis.App.Services.Applications;

namespace SATH.Aitisis.App.Services.AiReview
{
    public sealed class ApplicationAiReviewService : IApplicationAiReviewService
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IAiReviewAssistant _aiReviewAssistant;

        public ApplicationAiReviewService(IApplicationRepository applicationRepository, IAiReviewAssistant aiReviewAssistant)
        {
            _applicationRepository = applicationRepository;
            _aiReviewAssistant = aiReviewAssistant;
        }


        public async Task<AiReviewResult> GenerateForApplicationAsync(int applicationId)
        {
            if (applicationId <= 0)
            {
                return AiReviewResult.Failure("Invalid application id.");
            }

            var application = await _applicationRepository.GetApplicationForReviewAsync(applicationId);

            if (application == null)
            {
                return AiReviewResult.Failure("The application was not found.");
            }

            if (application.ApplicationType == null)
            {
                return AiReviewResult.Failure("The application type could not be loaded.");
            }

            var fields = application.FieldValues
                .Where(x => x.ApplicationTypeField != null)
                .OrderBy(x => x.ApplicationTypeField.DisplayOrder)
                .Select(x => new AiReviewFieldItem
                {
                    Label = x.ApplicationTypeField.Label,
                    Value = string.IsNullOrWhiteSpace(x.Value)
                        ? "-"
                        : x.Value,
                    DisplayOrder = x.ApplicationTypeField.DisplayOrder
                })
                .ToArray();

            if (fields.Length == 0)
            {
                return AiReviewResult.Failure("There are no submitted fields available for AI review.");
            }

            var request = new AiReviewRequest
            {
                ApplicationId = application.Id,
                ApplicationTypeName = application.ApplicationType.Name,
                Fields = fields
            };

            return await _aiReviewAssistant.GenerateReviewAsync(request);
        }
    }
}
