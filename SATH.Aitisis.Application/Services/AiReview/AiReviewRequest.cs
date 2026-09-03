namespace SATH.Aitisis.App.Services.AiReview
{
    public sealed class AiReviewRequest
    {
        public int ApplicationId { get; init; }

        public string ApplicationTypeName { get; init; } = string.Empty;

        public IReadOnlyCollection<AiReviewFieldItem> Fields { get; init; } = Array.Empty<AiReviewFieldItem>();
    }
}
