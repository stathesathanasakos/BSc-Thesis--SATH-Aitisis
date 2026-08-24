namespace SATH.Aitisis.Web.Areas.Admin.ViewModels.Dashboard
{
    public sealed class AdminDashboardViewModel
    {
        public int TotalApplications { get; set; }

        public int PendingApplications { get; set; }

        public int ApprovedApplications { get; set; }

        public int RejectedApplications { get; set; }

        public int ActiveApplicationTypes { get; set; }
    }
}