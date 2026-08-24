namespace SATH.Aitisis.App.Services.Applications
{
    public sealed class ApplicationDecisionResult
    {
        public bool Succeeded { get; init; }

        public string? ErrorMessage { get; init; }

        public string? ProtocolNumber { get; init; }

        public static ApplicationDecisionResult Success(string? protocolNumber = null)
        {
            return new ApplicationDecisionResult
            {
                Succeeded = true,
                ProtocolNumber = protocolNumber
            };
        }

        public static ApplicationDecisionResult Failure(string errorMessage)
        {
            return new ApplicationDecisionResult
            {
                Succeeded = false,
                ErrorMessage = errorMessage
            };
        }
    }
}