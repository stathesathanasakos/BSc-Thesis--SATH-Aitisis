namespace SATH.Aitisis.App.Services.Employees
{
    public sealed class CreateEmployeeResult
    {
        public bool Succeeded { get; init; }

        public IReadOnlyCollection<string> Errors { get; init; }
            = Array.Empty<string>();
    }
}