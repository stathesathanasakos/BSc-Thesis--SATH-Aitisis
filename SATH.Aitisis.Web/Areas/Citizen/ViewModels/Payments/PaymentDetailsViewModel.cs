namespace SATH.Aitisis.Web.Areas.Citizen.ViewModels.Payments
{
    public sealed class PaymentDetailsViewModel
    {
        public int ApplicationId { get; set; }

        public string ApplicationTypeName { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string Currency { get; set; } = "eur";

        public string PaymentStatusText { get; set; } = "Not started";

        public bool RequiresPayment { get; set; }

        public bool CanStartPayment { get; set; }
    }
}