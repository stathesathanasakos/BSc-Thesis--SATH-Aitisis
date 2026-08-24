using Microsoft.EntityFrameworkCore;
using SATH.Aitisis.App.Services.DashboardStatistics;
using SATH.Aitisis.Domain.Enums;
using SATH.Aitisis.Infrastructure.Persistence;


namespace SATH.Aitisis.Infrastructure.Repositories.DashboardStatistics
{
    public sealed class DashboardStatisticsRepository : IDashboardStatisticsRepository
    {
        private readonly SathAitisisDbContext _dbContext;

        public DashboardStatisticsRepository(SathAitisisDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<AdminDashboardStatistics> GetAdminStatisticsAsync()
        {
            var totalApplications =
                await _dbContext.Applications.CountAsync();

            var pendingApplications =
                await _dbContext.Applications.CountAsync(x =>
                    x.Status == ApplicationStatus.Submitted);

            var approvedApplications =
                await _dbContext.Applications.CountAsync(x =>
                    x.Status == ApplicationStatus.Approved);

            var rejectedApplications =
                await _dbContext.Applications.CountAsync(x =>
                    x.Status == ApplicationStatus.Rejected);

            var activeApplicationTypes =
                await _dbContext.ApplicationTypes.CountAsync(x =>
                    x.IsActive);

            return new AdminDashboardStatistics
            {
                TotalApplications = totalApplications,
                PendingApplications = pendingApplications,
                ApprovedApplications = approvedApplications,
                RejectedApplications = rejectedApplications,
                ActiveApplicationTypes = activeApplicationTypes
            };
        }

        public async Task<EmployeeDashboardStatistics> GetEmployeeStatisticsAsync(string employeeId)
        {
            var pendingApplications =
                await _dbContext.Applications.CountAsync(x =>
                    x.Status == ApplicationStatus.Submitted);

            var approvedByMe =
                await _dbContext.Applications.CountAsync(x =>
                    x.Status == ApplicationStatus.Approved &&
                    x.ReviewedByUserId == employeeId);

            var rejectedByMe =
                await _dbContext.Applications.CountAsync(x =>
                    x.Status == ApplicationStatus.Rejected &&
                    x.ReviewedByUserId == employeeId);

            return new EmployeeDashboardStatistics
            {
                PendingApplications = pendingApplications,
                ApprovedByMe = approvedByMe,
                RejectedByMe = rejectedByMe,
                TotalDecisionsByMe = approvedByMe + rejectedByMe
            };
        }

        public async Task<CitizenDashboardStatistics> GetCitizenStatisticsAsync(string citizenId)
        {
            var totalApplications =
                await _dbContext.Applications.CountAsync(x =>
                    x.CitizenId == citizenId);

            var pendingApplications =
                await _dbContext.Applications.CountAsync(x =>
                    x.CitizenId == citizenId &&
                    x.Status == ApplicationStatus.Submitted);

            var approvedApplications =
                await _dbContext.Applications.CountAsync(x =>
                    x.CitizenId == citizenId &&
                    x.Status == ApplicationStatus.Approved);

            var rejectedApplications =
                await _dbContext.Applications.CountAsync(x =>
                    x.CitizenId == citizenId &&
                    x.Status == ApplicationStatus.Rejected);

            return new CitizenDashboardStatistics
            {
                TotalApplications = totalApplications,
                PendingApplications = pendingApplications,
                ApprovedApplications = approvedApplications,
                RejectedApplications = rejectedApplications
            };
        }
    }
}