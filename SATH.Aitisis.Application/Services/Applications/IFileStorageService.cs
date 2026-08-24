namespace SATH.Aitisis.App.Services.Applications
{
    public interface IFileStorageService
    {
        Task<string> SaveAsync(Stream content, string originalFileName);

        Task<Stream?> OpenReadAsync(string storedFileName);
    }
}