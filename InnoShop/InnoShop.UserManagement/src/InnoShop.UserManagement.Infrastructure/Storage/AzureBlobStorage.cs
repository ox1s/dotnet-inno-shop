using Azure.Storage.Blobs;
using InnoShop.UserManagement.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace InnoShop.UserManagement.Infrastructure.Storage;

public class AzureBlobStorage(
    BlobServiceClient client,
    ILogger<AzureBlobStorage> logger) : IFileStorage
{
    public async Task<string> UploadAsync(Stream stream, string fileName, string contentType,
        CancellationToken cancellationToken = default)
    {
        var blobContainerClient = client.GetBlobContainerClient("user-images");
        var blobClient = blobContainerClient.GetBlobClient(fileName);
        try
        {
            await blobClient.UploadAsync(stream, cancellationToken: cancellationToken);

            logger.LogInformation("Uploaded file {FileName} to blob {BlobName}", fileName, blobClient.Name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error uploading file {FileName}", fileName);
            throw;
        }

        return blobClient.Uri.ToString();
    }

    public async Task DeleteAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        var blobContainerClient = client.GetBlobContainerClient("user-images");
        var blobClient = blobContainerClient.GetBlobClient(fileUrl);

        try
        {
            await blobClient.DeleteAsync(cancellationToken: cancellationToken);
            
            logger.LogInformation("Deleted file {FileName} from blob {BlobName}", fileUrl, blobClient.Name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting file {FileName}", fileUrl);
            throw;
        }
    }
}