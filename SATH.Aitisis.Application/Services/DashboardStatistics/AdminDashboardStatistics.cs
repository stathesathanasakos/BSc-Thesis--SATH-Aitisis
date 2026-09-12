namespace SATH.Aitisis.App.Services.DashboardStatistics
{
    public sealed class AdminDashboardStatistics
    {
        public int TotalApplications { get; init; }

        public int PendingApplications { get; init; }

        public int ApprovedApplications { get; init; }

        public int RejectedApplications { get; init; }

        public int ActiveApplicationTypes { get; init; }

        // for the last feature which is to show the most popular applicatyion type
        public string? MostPopularApplicationTypeName { get; init; }

        public int MostPopularApplicationTypeCount { get; init; }

        public decimal MostPopularApplicationTypePercentage { get; init; }

        public int SubmittedApplicationsLastMonth { get; init; }
    }
}