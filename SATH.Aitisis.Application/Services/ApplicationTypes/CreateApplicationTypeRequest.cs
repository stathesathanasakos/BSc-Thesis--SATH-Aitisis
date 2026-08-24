namespace SATH.Aitisis.App.Services.ApplicationTypes
{
    public sealed class CreateApplicationTypeRequest
    {
        public string Name { get; init; } = string.Empty;

        public string? Description { get; init; }

        public bool RequiresPayment { get; init; }

        public decimal? FeeAmount { get; init; }
    }
}