namespace SATH.Aitisis.App.Services.Applications
{
    public interface IApplicationService
    {
        Task<IReadOnlyCollection<AvailableApplicationTypeItem>> GetAvailableApplicationTypesAsync();

        Task<ApplicationSubmissionForm?> GetSubmissionFormAsync(int applicationTypeId);

        Task<SubmitApplicationResult> SubmitApplicationAsync(SubmitApplicationRequest request);

        Task<UploadApplicationDocumentResult> UploadDocumentAsync(UploadApplicationDocumentRequest request);

        Task<ApplicationDocumentsInfo?> GetApplicationDocumentsInfoAsync(int applicationId, string citizenId);

        Task<CompleteApplicationSubmissionResult> CompleteSubmissionAsync(CompleteApplicationSubmissionRequest request);

        Task<bool> DeleteDraftApplicationAsync(int applicationId, string citizenId);

        Task<int> DeleteExpiredDraftApplicationsAsync(TimeSpan draftLifetime);

        Task<IReadOnlyCollection<ApplicationHistoryItem>> GetCitizenApplicationsAsync(string citizenId);

        // for checking the application by an employee
        Task<IReadOnlyCollection<PendingApplicationItem>> GetPendingApplicationsAsync();

        Task<ApplicationReviewDetails?> GetApplicationForReviewAsync(int applicationId);

        Task<ApplicationDocumentFile?> GetDocumentFileAsync(int documentId);

        Task<ApplicationDecisionResult> ApproveApplicationAsync(ApproveApplicationRequest request);

        Task<ApplicationDecisionResult> RejectApplicationAsync(RejectApplicationRequest request);

        Task<ApplicationTimelineDetails?> GetCitizenApplicationTimelineAsync(int applicationId, string citizenId);

        Task<ApplicationTimelineDetails?> GetEmployeeApplicationTimelineAsync(int applicationId);
    }
}