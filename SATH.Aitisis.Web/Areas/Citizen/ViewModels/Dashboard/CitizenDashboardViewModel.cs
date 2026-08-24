namespace SATH.Aitisis.Web.Areas.Citizen.ViewModels.Dashboard
{
    public sealed class CitizenDashboardViewModel
    {
        public int TotalApplications { get; set; }

        public int PendingApplications { get; set; }

        public int ApprovedApplications { get; set; }

        public int RejectedApplications { get; set; }
    }
}