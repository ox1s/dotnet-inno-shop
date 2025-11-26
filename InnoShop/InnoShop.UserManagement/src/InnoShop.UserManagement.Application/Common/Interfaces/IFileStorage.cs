namespace InnoShop.UserManagement.Application.Common.Interfaces;

public interface IFileStorage
{
    Task<string> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task DeleteAsync(string fileUrl, CancellationToken cancellationToken = default);
}