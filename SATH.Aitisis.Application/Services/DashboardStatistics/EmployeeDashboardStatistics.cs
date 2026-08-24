namespace SATH.Aitisis.App.Services.DashboardStatistics
{
    public sealed class EmployeeDashboardStatistics
    {
        public int PendingApplications { get; init; }

        public int ApprovedByMe { get; init; }

        public int RejectedByMe { get; init; }

        public int TotalDecisionsByMe { get; init; }
    }
}