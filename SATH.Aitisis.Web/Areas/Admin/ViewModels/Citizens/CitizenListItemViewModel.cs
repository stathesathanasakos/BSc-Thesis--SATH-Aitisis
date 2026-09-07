namespace SATH.Aitisis.Web.Areas.Admin.ViewModels.Citizens
{
    public sealed class CitizenListViewModel
    {
        public string Id { get; init; } = string.Empty;

        public string FirstName { get; init; } = string.Empty;

        public string LastName { get; init; } = string.Empty;

        public string Email { get; init; } = string.Empty;

        public bool IsActive { get; init; }

        public DateTime CreatedAt { get; init; }
    }
}