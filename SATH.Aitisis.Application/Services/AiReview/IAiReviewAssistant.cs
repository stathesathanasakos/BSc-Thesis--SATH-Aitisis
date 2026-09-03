namespace SATH.Aitisis.App.Services.AiReview
{
    public interface IAiReviewAssistant
    {
        Task<AiReviewResult> GenerateReviewAsync(AiReviewRequest request);
    }
}
