namespace SATH.Aitisis.App.Services.DocumentGuidance
{
    public interface IApplicationDocumentGuidanceService
    {
        Task<DocumentGuidanceResult> GenerateForApplicationAsync(int applicationId, string citizenId);
    }
}