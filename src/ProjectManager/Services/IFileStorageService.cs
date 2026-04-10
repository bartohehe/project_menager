using System.IO;

namespace ProjectManager.Services;

public interface IFileStorageService
{
    Task<string> UploadFileAsync(Stream stream, string fileName, string contentType, string projectId, string category);
    Task<byte[]> DownloadFileAsync(string fileId);
    Task DeleteFileAsync(string fileId);
    Task DeleteAllForProjectAsync(string projectId);
}
