namespace SATH.Aitisis.App.Services.ApprovalCertificates
{
    public interface IApprovalCertificatePdfGenerator
    {
        byte[] Generate(ApprovalCertificateData data);
    }
}