namespace SATH.Aitisis.Web.Areas.Admin.ViewModels.Dashboard
{
    public sealed class AdminDashboardViewModel
    {
        public int TotalApplications { get; set; }

        public int PendingApplications { get; set; }

        public int ApprovedApplications { get; set; }

        public int RejectedApplications { get; set; }

        public int ActiveApplicationTypes { get; set; }

        // for the last feature which is to show the most popular application type
        public string? MostPopularApplicationTypeName { get; set; }

        public int MostPopularApplicationTypeCount { get; set; }

        public decimal MostPopularApplicationTypePercentage { get; set; }

        public int SubmittedApplicationsLastMonth { get; set; }
    }
}