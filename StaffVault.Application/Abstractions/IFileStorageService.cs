namespace StaffVault.Application.Abstractions
{
    public interface IFileStorageService
    {
        Task<string?> UploadFileAsync(
            Stream content,
            string fileName,
            CancellationToken cancellationToken = default
        );
        void DeleteFile(string? storedPath);
    }
}
