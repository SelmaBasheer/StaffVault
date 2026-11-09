using StaffVault.Application.Abstractions;

namespace StaffVault.Infrastructure.Storage;

/// Saves employee pictures to wwwroot/uploads.
internal sealed class LocalFileStorageService(string uploadsFolderPath) : IFileStorageService
{
    private const string UploadsUrl = "/uploads";

    public async Task<string> SaveAsync(
        Stream content,
        string originalFileName,
        CancellationToken cancellationToken = default
    )
    {
        // Our own unique name: avoids overwriting and unsafe user file names.
        var extension = Path.GetExtension(originalFileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid():N}{extension}";

        Directory.CreateDirectory(uploadsFolderPath);
        var fullPath = Path.Combine(uploadsFolderPath, fileName);

        await using var fileStream = File.Create(fullPath);
        await content.CopyToAsync(fileStream, cancellationToken);

        return $"/uploads/{fileName}";
    }

    public void DeleteFile(string? storedPath)
    {
        if (string.IsNullOrWhiteSpace(storedPath))
        {
            return;
        }

        // GetFileName keeps only "abc.png", so a path can never point outside the uploads folder.
        var fullPath = Path.Combine(uploadsFolderPath, Path.GetFileName(storedPath));

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }
}
