namespace SATH.Aitisis.Web.Areas.Employee.ViewModels.Dashboard
{
    public sealed class EmployeeDashboardViewModel
    {
        public int PendingApplications { get; set; }

        public int ApprovedByMe { get; set; }

        public int RejectedByMe { get; set; }

        public int TotalDecisionsByMe { get; set; }
    }
}
