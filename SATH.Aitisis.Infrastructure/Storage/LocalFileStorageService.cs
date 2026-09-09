using SATH.Aitisis.App.Services.Applications;

namespace SATH.Aitisis.Infrastructure.Storage
{
    public sealed class LocalFileStorageService : IFileStorageService
    {
        private readonly string _storagePath;

        public LocalFileStorageService()
        {
            _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "Storage", "ApplicationDocuments");
            Directory.CreateDirectory(_storagePath);
        }

        public async Task<string> SaveAsync(Stream content, string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName);

            var storedFileName =$"{Guid.NewGuid():N}{extension}";

            var fullPath = Path.Combine( _storagePath, storedFileName);

            await using var fileStream = new FileStream(
                fullPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None);

            await content.CopyToAsync(fileStream);

            return storedFileName;
        }



        public Task<Stream?> OpenReadAsync(string storedFileName)
        {
            if (string.IsNullOrWhiteSpace(storedFileName))
            {
                return Task.FromResult<Stream?>(null);
            }

            var safeFileName = Path.GetFileName(storedFileName);

            var filePath = Path.Combine(_storagePath, safeFileName);

            if (!File.Exists(filePath))
            {
                return Task.FromResult<Stream?>(null);
            }

            Stream stream = new FileStream( filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            return Task.FromResult<Stream?>(stream);
        }



        // to delete a file belongs to a draft applicartion from the storage
        public Task DeleteAsync(string storedFileName)
        {
            if (string.IsNullOrWhiteSpace(storedFileName))
            {
                return Task.CompletedTask;
            }

            var safeFileName = Path.GetFileName(storedFileName);
            var filePath = Path.Combine(_storagePath, safeFileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            return Task.CompletedTask;
        }
    }
}