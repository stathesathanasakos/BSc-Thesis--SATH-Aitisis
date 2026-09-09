namespace SATH.Aitisis.App.Services.Applications
{
    public interface IFileStorageService
    {
        Task<string> SaveAsync(Stream content, string originalFileName);

        Task<Stream?> OpenReadAsync(string storedFileName);

        // to delete a file belongs to a draft applicartion from the storage
        Task DeleteAsync(string storedFileName);
    }
}