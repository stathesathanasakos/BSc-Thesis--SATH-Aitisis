namespace SATH.Aitisis.App.Services.DocumentGuidance
{
    public interface ICitizenDocumentGuidanceAssistant
    {
        Task<DocumentGuidanceResult> GenerateGuidanceAsync(DocumentGuidanceRequest request);
    }
}