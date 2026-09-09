using Microsoft.EntityFrameworkCore;
using SATH.Aitisis.App.Services.Applications;
using SATH.Aitisis.Domain.Entities;
using SATH.Aitisis.Domain.Enums;
using SATH.Aitisis.Infrastructure.Persistence;
using DomainApplication = SATH.Aitisis.Domain.Entities.Application;
using DomainDocument = SATH.Aitisis.Domain.Entities.Document;


namespace SATH.Aitisis.Infrastructure.Repositories.Applications
{
    public sealed class ApplicationRepository : IApplicationRepository
    {
        private readonly SathAitisisDbContext _dbContext;

        public ApplicationRepository(SathAitisisDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyCollection<ApplicationType>> GetActiveApplicationTypesAsync()
        {
            return await _dbContext.ApplicationTypes
                .AsNoTracking()
                .Where(x => x.IsActive)
                .ToArrayAsync();
        }

        public async Task<ApplicationType?> GetApplicationTypeWithConfigurationAsync(int applicationTypeId)
        {
            return await _dbContext.ApplicationTypes
                .AsNoTracking()
                .Include(x => x.Fields)
                .Include(x => x.DocumentRequirements)
                .FirstOrDefaultAsync(x => x.Id == applicationTypeId);
        }

        public async Task AddApplicationAsync(DomainApplication application)
        {
            await _dbContext.Applications.AddAsync(application);
        }

        public async Task AddFieldValuesAsync(IEnumerable<ApplicationFieldValue> fieldValues)
        {
            await _dbContext.ApplicationFieldValues.AddRangeAsync(fieldValues);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }


        public async Task AddDocumentAsync(Document document)
        {
            await _dbContext.Documents.AddAsync(document);
        }



        public async Task<IReadOnlyCollection<Document>> GetApplicationDocumentsAsync(int applicationId)
        {
            return await _dbContext.Documents
                .AsNoTracking()
                .Where(x => x.ApplicationId == applicationId)
                .ToArrayAsync();
        }


        public async Task<IReadOnlyCollection<DomainApplication>> GetByCitizenIdAsync(string citizenId)
        {
            return await _dbContext.Applications
                .AsNoTracking()
                .Include(x => x.ApplicationType)
                .Include(x => x.Payment)
                .Where(x =>
                    x.CitizenId == citizenId &&
                        (
                            x.Status == ApplicationStatus.Submitted ||
                            x.Status == ApplicationStatus.Approved ||
                            x.Status == ApplicationStatus.Rejected
                        )
                 )
                .OrderByDescending(x => x.SubmissionDate)
                .ToListAsync();

        }


        // for the employee to get the pending applications
        public async Task<IReadOnlyCollection<DomainApplication>>GetPendingApplicationsAsync()
        {
            // I need only the application that are submitted and if the application type requires payment, then the payment must be completed
            return await _dbContext.Applications
                .AsNoTracking()
                .Include(x => x.ApplicationType)
                .Where(x =>
                    x.Status == ApplicationStatus.Submitted &&
                        (
                            !x.ApplicationType.RequiresPayment ||
                        (
                            x.Payment != null &&
                            x.Payment.Status == PaymentStatus.Completed
                        )
                ))
                .OrderBy(x => x.SubmissionDate)
                .ToArrayAsync();
        }


        public async Task<DomainDocument?> GetDocumentByIdAsync(int documentId)
        {
            return await _dbContext.Documents.AsNoTracking().FirstOrDefaultAsync(x => x.Id == documentId);
        }



        public async Task<DomainApplication?> GetByIdForUpdateAsync(int applicationId)
        {
            return await _dbContext.Applications.Include(x => x.ApplicationType).FirstOrDefaultAsync(x => x.Id == applicationId);
        }


        // to get the approved application for certificate generation
        public async Task<DomainApplication?>GetApprovedApplicationForCertificateAsync(int applicationId, string citizenId)
        {
            return await _dbContext.Applications
                .AsNoTracking()
                .Include(x => x.ApplicationType)
                .Include(x => x.FieldValues).ThenInclude(x => x.ApplicationTypeField)
                .FirstOrDefaultAsync(x =>
                    x.Id == applicationId &&
                    x.CitizenId == citizenId &&
                    x.Status == ApplicationStatus.Approved &&
                    x.DecisionDate.HasValue &&
                    x.ProtocolNumber != null &&
                    x.ReviewedByUserId != null);
        }



        public async Task<DomainApplication?> GetApplicationForDocumentUploadAsync(int applicationId, string citizenId)
        {
            return await _dbContext.Applications
                .Include(x => x.ApplicationType)
                    .ThenInclude(x => x.DocumentRequirements)
                .Include(x => x.Documents)
                .FirstOrDefaultAsync(x =>
                    x.Id == applicationId &&
                    x.CitizenId == citizenId &&
                    x.Status == ApplicationStatus.Draft);
        }

        public async Task<DomainApplication?> GetApplicationForSubmissionCompletionAsync(int applicationId, string citizenId)
        {
            return await _dbContext.Applications
                .Include(x => x.ApplicationType)
                    .ThenInclude(x => x.DocumentRequirements)
                .Include(x => x.Documents)
                .Include(x => x.Payment)
                .FirstOrDefaultAsync(x =>
                    x.Id == applicationId &&
                    x.CitizenId == citizenId);
        }

        // to get the draft application for delete, to be used by the citizen
        public async Task<DomainApplication?> GetDraftApplicationForDeleteAsync(int applicationId, string citizenId)
        {
            return await _dbContext.Applications
                .Include(x => x.Documents)
                .Include(x => x.FieldValues)
                .Include(x => x.Payment)
                .FirstOrDefaultAsync(x =>
                    x.Id == applicationId &&
                    x.CitizenId == citizenId &&
                    x.Status == ApplicationStatus.Draft);
        }


        // to get all the draft applications that are older than a certain date, to be deleted by a background job
        public async Task<IReadOnlyCollection<DomainApplication>> GetDraftApplicationsOlderThanAsync(DateTime cutoffUtc)
        {
            return await _dbContext.Applications
                .Include(x => x.Documents)
                .Include(x => x.FieldValues)
                .Include(x => x.Payment)
                .Where(x =>
                    x.Status == ApplicationStatus.Draft &&
                    x.SubmissionDate < cutoffUtc)
                .ToListAsync();
        }

        // to delete an application and all its related entities, to be used by the citizen or by a background job
        public void DeleteApplication(DomainApplication application)
        {
            if (application.Payment != null)
            {
                _dbContext.Payments.Remove(application.Payment);
            }

            if (application.FieldValues.Count > 0)
            {
                _dbContext.ApplicationFieldValues.RemoveRange(application.FieldValues);
            }

            if (application.Documents.Count > 0)
            {
                _dbContext.Documents.RemoveRange(application.Documents);
            }

            _dbContext.Applications.Remove(application);
        }


        // to delete multiple draft applications and all their related entities, to be used by a background job
        public void DeleteApplications(IEnumerable<DomainApplication> applications)
        {
            foreach (var application in applications)
            {
                DeleteApplication(application);
            }
        }


        public async Task<DomainApplication?> GetApplicationForReviewAsync(int applicationId)
        {
            return await _dbContext.Applications
                .AsNoTracking()
                .Include(x => x.ApplicationType)
                .Include(x => x.FieldValues)
                    .ThenInclude(x => x.ApplicationTypeField)
                .Include(x => x.Documents)
                    .ThenInclude(x => x.DocumentRequirement)
                .FirstOrDefaultAsync(x =>
                    x.Id == applicationId &&
                    x.Status == ApplicationStatus.Submitted);
        }


        // to use for the timeline of each application for citizen
        public async Task<DomainApplication?> GetTimelineForCitizenAsync(int applicationId, string citizenId)
        {
            return await _dbContext.Applications
                .AsNoTracking()
                .Include(x => x.ApplicationType)
                .Include(x => x.Documents)
                    .ThenInclude(x => x.DocumentRequirement)
                .FirstOrDefaultAsync(x =>
                    x.Id == applicationId &&
                    x.CitizenId == citizenId &&
                    x.Status != ApplicationStatus.Draft);
        }

        // to use for the timeline of each application for employee
        public async Task<DomainApplication?> GetTimelineForEmployeeAsync(int applicationId)
        {
            return await _dbContext.Applications
                .AsNoTracking()
                .Include(x => x.ApplicationType)
                .Include(x => x.Documents)
                    .ThenInclude(x => x.DocumentRequirement)
                .FirstOrDefaultAsync(x =>
                    x.Id == applicationId &&
                    x.Status != ApplicationStatus.Draft);
        }
    }
}