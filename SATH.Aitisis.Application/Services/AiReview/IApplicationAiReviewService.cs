namespace SATH.Aitisis.App.Services.AiReview
{
    public interface IApplicationAiReviewService
    {
        Task<AiReviewResult> GenerateForApplicationAsync(int applicationId);
    }
}
