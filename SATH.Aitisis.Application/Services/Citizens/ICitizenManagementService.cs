namespace SATH.Aitisis.App.Services.Citizens
{
    public interface ICitizenManagementService
    {
        Task<IReadOnlyCollection<CitizenListItem>> GetCitizensAsync();

        Task<bool> ReactivateCitizenAsync(string citizenId);
    }
}