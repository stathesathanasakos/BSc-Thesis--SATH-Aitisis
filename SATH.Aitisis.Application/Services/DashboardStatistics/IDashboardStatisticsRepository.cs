namespace SATH.Aitisis.App.Services.DashboardStatistics
{
    public interface IDashboardStatisticsRepository
    {
        Task<AdminDashboardStatistics> GetAdminStatisticsAsync();

        Task<EmployeeDashboardStatistics> GetEmployeeStatisticsAsync(string employeeId);

        Task<CitizenDashboardStatistics> GetCitizenStatisticsAsync(string citizenId);
    }
}