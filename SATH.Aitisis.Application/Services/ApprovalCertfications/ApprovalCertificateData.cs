namespace SATH.Aitisis.App.Services.ApprovalCertificates
{
    public sealed class ApprovalCertificateData
    {
        public int ApplicationId { get; init; }

        public string ProtocolNumber { get; init; }= string.Empty;

        public string ApplicationTypeName { get; init; } = string.Empty;

        public DateTime SubmissionDate{ get; init; }

        public DateTime ApprovalDate { get; init; }

        public string CitizenFullName { get; init;}  = string.Empty;

        public string CitizenEmail { get; init; } = string.Empty;

        public string EmployeeFullName { get; init; } = string.Empty;

        public IReadOnlyCollection<ApprovalCertificateFieldItem> Fields { get; init; } = Array.Empty<ApprovalCertificateFieldItem>();
    }


    public sealed class ApprovalCertificateFieldItem
    {
        public string Label { get; init; } = string.Empty;

        public string Value { get; init; } = string.Empty;

        public int DisplayOrder { get; init; }
    }
}
