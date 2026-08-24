namespace SATH.Aitisis.App.Common.Users
{
    public interface IUserLookupService
    {
        Task<UserProfileInfo?> GetByIdAsync(string userId);
    }
}