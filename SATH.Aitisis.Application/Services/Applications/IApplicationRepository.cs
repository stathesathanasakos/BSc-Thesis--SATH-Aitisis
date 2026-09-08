using SATH.Aitisis.Domain.Entities;
using DomainApplication = SATH.Aitisis.Domain.Entities.Application;
using DomainDocument = SATH.Aitisis.Domain.Entities.Document;

namespace SATH.Aitisis.App.Services.Applications
{
    public interface IApplicationRepository
    {
        Task<IReadOnlyCollection<ApplicationType>> GetActiveApplicationTypesAsync();

        Task<ApplicationType?> GetApplicationTypeWithConfigurationAsync(int applicationTypeId);

        Task AddApplicationAsync(Application application);

        Task AddFieldValuesAsync(IEnumerable<ApplicationFieldValue> fieldValues);

        Task SaveChangesAsync();

        Task AddDocumentAsync(Document document);

        Task<DomainApplication?> GetApplicationForDocumentUploadAsync(int applicationId, string citizenId);

        Task<IReadOnlyCollection<Document>> GetApplicationDocumentsAsync(int applicationId);

        Task<DomainApplication?> GetApplicationForSubmissionCompletionAsync(int applicationId, string citizenId);

        Task<DomainApplication?> GetDraftApplicationForDeleteAsync(int applicationId, string citizenId);

        void DeleteApplication(DomainApplication application);

        Task<IReadOnlyCollection<DomainApplication>> GetByCitizenIdAsync(string citizenId);

        // for the employee to get the pending applications
        Task<IReadOnlyCollection<DomainApplication>> GetPendingApplicationsAsync();

        Task<DomainApplication?> GetApplicationForReviewAsync(int applicationId);

        Task<DomainDocument?> GetDocumentByIdAsync(int documentId);

        Task<DomainApplication?> GetByIdForUpdateAsync(int applicationId);

        // for the timeline of each application
        Task<DomainApplication?> GetTimelineForCitizenAsync(int applicationId, string citizenId);

        Task<DomainApplication?> GetTimelineForEmployeeAsync(int applicationId);

        // to get the approved application for certificate generation
        Task<DomainApplication?> GetApprovedApplicationForCertificateAsync(int applicationId, string citizenId);
    }
}