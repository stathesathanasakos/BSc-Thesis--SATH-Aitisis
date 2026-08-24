namespace SATH.Aitisis.App.Services.DashboardStatistics
{
    public sealed class CitizenDashboardStatistics
    {
        public int TotalApplications { get; init; }

        public int PendingApplications { get; init; }

        public int ApprovedApplications { get; init; }

        public int RejectedApplications { get; init; }
    }
}