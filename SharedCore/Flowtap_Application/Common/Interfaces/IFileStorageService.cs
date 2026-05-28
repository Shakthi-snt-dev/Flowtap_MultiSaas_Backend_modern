namespace Flowtap_Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<string> UploadAsync(Stream stream, string fileName, string contentType, string folder = "uploads", CancellationToken ct = default);
    Task DeleteAsync(string fileUrl, CancellationToken ct = default);
    string GetUrl(string filePath);
}
