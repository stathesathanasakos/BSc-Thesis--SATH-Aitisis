namespace SATH.Aitisis.App.Services.DashboardStatistics
{
    public sealed class AdminDashboardStatistics
    {
        public int TotalApplications { get; init; }

        public int PendingApplications { get; init; }

        public int ApprovedApplications { get; init; }

        public int RejectedApplications { get; init; }

        public int ActiveApplicationTypes { get; init; }
    }
}