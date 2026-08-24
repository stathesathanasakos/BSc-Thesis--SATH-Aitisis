namespace SATH.Aitisis.App.Services.ApprovalCertificates
{
    public interface IApprovalCertificateService
    {
        Task<ApprovalCertificateFile?> GenerateForCitizenAsync(int applicationId, string citizenId);
    }
}