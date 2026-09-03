namespace SATH.Aitisis.App.Services.AiReview
{
    public sealed class AiReviewFieldItem
    {
        // use it for the labels of the user's input fields
        public string Label { get; init; } = string.Empty;

        public string Value { get; init; } = string.Empty;

        public int DisplayOrder { get; init; }
    }
}