namespace StaffVault.Application.Abstractions
{
    public interface IFileStorageService
    {
        Task<string> SaveAsync(
            Stream content,
            string fileName,
            CancellationToken cancellationToken = default
        );
        void DeleteFile(string? storedPath);
    }
}
