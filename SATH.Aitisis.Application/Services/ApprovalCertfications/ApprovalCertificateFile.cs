namespace SATH.Aitisis.App.Services.ApprovalCertificates
{
    public sealed class ApprovalCertificateFile
    {
        public byte[] Content { get; init; } = Array.Empty<byte>();

        public string ContentType { get; init; } = "application/pdf";

        public string FileName { get; init; } = string.Empty;
    }
}