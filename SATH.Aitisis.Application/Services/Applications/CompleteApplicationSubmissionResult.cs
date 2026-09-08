namespace SATH.Aitisis.App.Services.Applications
{
    public sealed class CompleteApplicationSubmissionResult
    {
        public bool Succeeded { get; init; }

        public string? ErrorMessage { get; init; }

        public int? ApplicationId { get; init; }

        public static CompleteApplicationSubmissionResult Success(int applicationId)
        {
            return new CompleteApplicationSubmissionResult
            {
                Succeeded = true,
                ApplicationId = applicationId
            };
        }

        public static CompleteApplicationSubmissionResult Failure(string errorMessage)
        {
            return new CompleteApplicationSubmissionResult
            {
                Succeeded = false,
                ErrorMessage = errorMessage
            };
        }
    }
}