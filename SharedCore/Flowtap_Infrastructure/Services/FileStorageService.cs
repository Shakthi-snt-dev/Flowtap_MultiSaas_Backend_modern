using Flowtap_Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Flowtap_Infrastructure.Services;

public class FileStorageService(IWebHostEnvironment env, ILogger<FileStorageService> logger) : IFileStorageService
{
    private string WebRootPath =>
        string.IsNullOrEmpty(env.WebRootPath)
            ? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
            : env.WebRootPath;

    public async Task<string> UploadAsync(Stream stream, string fileName, string contentType, string folder = "uploads", CancellationToken ct = default)
    {
        var uploadsDir = Path.Combine(WebRootPath, folder);
        Directory.CreateDirectory(uploadsDir);

        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(fileName)}";
        var filePath = Path.Combine(uploadsDir, uniqueFileName);

        await using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        await stream.CopyToAsync(fileStream, ct);

        var relativeUrl = $"/{folder}/{uniqueFileName}";
        logger.LogInformation("File uploaded: {RelativeUrl}", relativeUrl);

        return relativeUrl;
    }

    public Task DeleteAsync(string fileUrl, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
            return Task.CompletedTask;

        // Convert relative URL to absolute path: /uploads/foo.jpg -> wwwroot/uploads/foo.jpg
        var relativePath = fileUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var absolutePath = Path.Combine(WebRootPath, relativePath);

        if (File.Exists(absolutePath))
        {
            File.Delete(absolutePath);
            logger.LogInformation("File deleted: {Path}", absolutePath);
        }
        else
        {
            logger.LogWarning("File not found for deletion: {Path}", absolutePath);
        }

        return Task.CompletedTask;
    }

    public string GetUrl(string filePath) => filePath;
}
