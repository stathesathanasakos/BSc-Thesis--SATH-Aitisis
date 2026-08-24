namespace SATH.Aitisis.App.Services.DashboardStatistics
{
    public interface IDashboardStatisticsService
    {
        Task<AdminDashboardStatistics> GetAdminStatisticsAsync();

        Task<EmployeeDashboardStatistics> GetEmployeeStatisticsAsync(string employeeId);

        Task<CitizenDashboardStatistics> GetCitizenStatisticsAsync(string citizenId);
    }
}
