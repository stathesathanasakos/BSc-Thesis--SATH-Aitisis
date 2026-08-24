namespace SATH.Aitisis.App.Services.ApplicationTypes
{
    public sealed class ApplicationTypeListItem
    {
        // it's a DTO (Data Transfer Object) that represents an item in a list of application types
        // I use it to transfer data between the application layer and the presentation layer (e.g., API or UI)
        public int Id { get; init; }

        public string Name { get; init; } = string.Empty;

        public string? Description { get; init; }

        public bool RequiresPayment { get; init; }

        public decimal? FeeAmount { get; init; }

        public bool IsActive { get; init; }

        public DateTime CreatedAt { get; init; }
    }
}