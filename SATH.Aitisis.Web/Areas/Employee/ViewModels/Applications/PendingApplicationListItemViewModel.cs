namespace SATH.Aitisis.Web.Areas.Employee.ViewModels.Applications
{
    public sealed class PendingApplicationListItemViewModel
    {
        public int Id { get; set; }

        public string ApplicationTypeName { get; set; } = string.Empty;

        public DateTime SubmissionDate { get; set; }

        public string StatusText { get; set; } = "Pending";
    }
}
