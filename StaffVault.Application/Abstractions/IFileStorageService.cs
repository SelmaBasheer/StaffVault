namespace StaffVault.Application.Abstractions
{
    //Stores employee pictures. Implemented in Infrastructure.
    public interface IFileStorageService
    {
        // he relative URL of the saved file, e.g. "/uploads/abc.png".
        Task<string> SaveAsync(
            Stream content,
            string fileName,
            CancellationToken cancellationToken = default
        );
        void DeleteFile(string? storedPath);
    }
}
