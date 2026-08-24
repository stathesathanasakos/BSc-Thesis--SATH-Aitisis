namespace SATH.Aitisis.App.Services.Applications
{
    public sealed class AvailableApplicationTypeItem
    {
        public int Id { get; init; }

        public string Name { get; init; } = string.Empty;

        public string? Description { get; init; }

        public bool RequiresPayment { get; init; }

        public decimal? FeeAmount { get; init; }
    }
}