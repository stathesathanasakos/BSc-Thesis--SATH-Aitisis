namespace SATH.Aitisis.App.Services.Applications
{
    public sealed class RejectApplicationRequest
    {
        public int ApplicationId { get; init; }

        public string EmployeeId { get; init; } = string.Empty;

        public string? EmployeeMessage { get; init; }
    }
}