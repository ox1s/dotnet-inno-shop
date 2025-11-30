using InnoShop.UserManagement.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace InnoShop.UserManagement.Infrastructure.Storage;

public class MinioFileStorage(
    IMinioClient minioClient,
    IOptions<BlobStorageSettings> settings,
    ILogger<MinioFileStorage> logger) : IFileStorage
{
    private string BucketName => settings.Value.BucketName;

    public async Task<string> UploadAsync(
        Stream stream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        await EnsureBucketExistsAsync(cancellationToken);

        var objectName = $"{Guid.NewGuid()}-{fileName}";

        stream.Seek(0, SeekOrigin.Begin);
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(BucketName)
            .WithObject(objectName)
            .WithStreamData(stream)
            .WithObjectSize(stream.Length)
            .WithContentType(contentType);

        try
        {
            await minioClient
                .PutObjectAsync(putObjectArgs, cancellationToken);

            logger.LogInformation("Uploaded file {ObjectName} to bucket {Bucket}", objectName, BucketName);
        }
        catch (MinioException ex)
        {
            logger.LogError(ex, $"Error uploading object '{objectName}'.");
            throw;
        }

        return objectName;
    }

    public async Task DeleteAsync(string fileName, CancellationToken cancellationToken = default)
    {
        try
        {
            var args = new RemoveObjectArgs()
                .WithBucket(BucketName)
                .WithObject(fileName);

            await minioClient
                .RemoveObjectAsync(args, cancellationToken);

            logger.LogInformation("Deleted file {FileName} from bucket {Bucket}", fileName, BucketName);
        }
        catch (MinioException ex)
        {
            logger.LogError(ex, $"Error deleting object '{fileName}'.");
            throw;
        }
    }

    private async Task EnsureBucketExistsAsync(CancellationToken cancellationToken)
    {
        var bucketExistsArgs = new BucketExistsArgs().WithBucket(BucketName);
        try
        {
            var found = await minioClient.BucketExistsAsync(bucketExistsArgs, cancellationToken);

            if (!found)
            {
                var makeBucketArgs = new MakeBucketArgs().WithBucket(BucketName);

                await minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);
            }

            var policy =
                $@"{{""Version"":""2012-10-17"",""Statement"":[{{""Action"":[""s3:GetBucketLocation""],""Effect"":""Allow"",""Principal"":{{""AWS"":[""*""]}},""Resource"":[""arn:aws:s3:::{BucketName}""],""Sid"":""""}},{{""Action"":[""s3:ListBucket""],""Condition"":{{""StringEquals"":{{""s3:prefix"":[""foo"",""prefix/""]}}}},""Effect"":""Allow"",""Principal"":{{""AWS"":[""*""]}},""Resource"":[""arn:aws:s3:::{BucketName}""],""Sid"":""""}},{{""Action"":[""s3:GetObject""],""Effect"":""Allow"",""Principal"":{{""AWS"":[""*""]}},""Resource"":[""arn:aws:s3:::{BucketName}/foo*"",""arn:aws:s3:::{BucketName}/prefix/*""],""Sid"":""""}}]}}";

            await minioClient
                .SetPolicyAsync(
                    new SetPolicyArgs()
                        .WithBucket(BucketName)
                        .WithPolicy(policy), cancellationToken);

            logger.LogInformation("Created bucket {Bucket} and set public policy", BucketName);
        }
        catch (MinioException e)
        {
            logger.LogInformation("Error occurred: " + e);
        }
    }

    public async Task<string> GetPresignedUrlForUploadAsync(string fileName, string contentType,
        CancellationToken cancellationToken = default)
    {
        await EnsureBucketExistsAsync(cancellationToken);

        var objectName = $"{Guid.NewGuid()}-{fileName}";

        var args = new PresignedPutObjectArgs()
            .WithBucket(BucketName)
            .WithObject(objectName)
            .WithExpiry(60 * 10);

        var presignedUrl = await minioClient.PresignedPutObjectAsync(args);

        return presignedUrl;
    }
}