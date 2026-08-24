namespace SATH.Aitisis.App.Services.DashboardStatistics
{
    public sealed class DashboardStatisticsService : IDashboardStatisticsService
    {
        private readonly IDashboardStatisticsRepository _repository;

        public DashboardStatisticsService(IDashboardStatisticsRepository repository)
        {
            _repository = repository;
        }

        public async Task<AdminDashboardStatistics> GetAdminStatisticsAsync()
        {
            return await _repository.GetAdminStatisticsAsync();
        }

        public async Task<EmployeeDashboardStatistics> GetEmployeeStatisticsAsync(string employeeId)
        {
            if (string.IsNullOrWhiteSpace(employeeId))
            {
                return new EmployeeDashboardStatistics();
            }

            return await _repository.GetEmployeeStatisticsAsync(employeeId);
        }

        public async Task<CitizenDashboardStatistics> GetCitizenStatisticsAsync(string citizenId)
        {
            if (string.IsNullOrWhiteSpace(citizenId))
            {
                return new CitizenDashboardStatistics();
            }

            return await _repository.GetCitizenStatisticsAsync(citizenId);
        }
    }
}