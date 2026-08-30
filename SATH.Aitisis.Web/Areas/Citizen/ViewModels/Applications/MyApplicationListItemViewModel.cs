using SATH.Aitisis.Domain.Enums;

namespace SATH.Aitisis.Web.Areas.Citizen.ViewModels.Applications
{
    public sealed class MyApplicationListItemViewModel
    {
        public int Id { get; set; }

        public string ApplicationTypeName { get; set; } = string.Empty;

        public string StatusText { get; set; } = string.Empty;

        public string StatusCssClass { get; set; } = string.Empty;

        public DateTime SubmissionDate { get; set; }

        public string? ProtocolNumber { get; set; }

        public DateTime? DecisionDate { get; set; }

        public bool RequiresPayment { get; set; }

        public PaymentStatus? PaymentStatus { get; set; }

        public bool CanPay { get; set; }
    }
}