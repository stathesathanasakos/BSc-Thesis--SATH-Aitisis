namespace SATH.Aitisis.App.Services.Employees
{
    public sealed class CreateEmployeeRequest
    {
        public string FirstName { get; init; } = string.Empty;

        public string LastName { get; init; } = string.Empty;

        public string Email { get; init; } = string.Empty;

        public string Password { get; init; } = string.Empty;
    }
}