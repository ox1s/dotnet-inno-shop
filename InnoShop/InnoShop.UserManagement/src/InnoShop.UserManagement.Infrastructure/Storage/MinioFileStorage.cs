using InnoShop.UserManagement.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace InnoShop.UserManagement.Infrastructure.Storage;

public class MinioFileStorage(
    IMinioClient _minioClient,
    ILogger<MinioFileStorage> _logger) : IFileStorage
{
    private const string BucketName = "user-avatars";

    public async Task<string> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken cancellationToken = default)
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
            await _minioClient
                .PutObjectAsync(putObjectArgs, cancellationToken);

            _logger.LogInformation("Uploaded file {ObjectName} to bucket {Bucket}", objectName, BucketName);
        }
        catch (MinioException ex)
        {
            _logger.LogError(ex, $"Error uploading object '{objectName}'.");
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

            await _minioClient
                .RemoveObjectAsync(args, cancellationToken);

            _logger.LogInformation("Deleted file {FileName} from bucket {Bucket}", fileName, BucketName);
        }
        catch (MinioException ex)
        {
            _logger.LogError(ex, $"Error deleting object '{fileName}'.");
            throw;
        }

    }

    private async Task EnsureBucketExistsAsync(CancellationToken cancellationToken)
    {
        var bucketExistsArgs = new BucketExistsArgs()
            .WithBucket(BucketName);

        bool found = await _minioClient
            .BucketExistsAsync(bucketExistsArgs, cancellationToken);

        if (!found)
        {
            var makeBucketArgs = new MakeBucketArgs()
                .WithBucket(BucketName);
            await _minioClient
                .MakeBucketAsync(makeBucketArgs, cancellationToken);
        }

        var policy = $@"{{""Version"":""2012-10-17"",""Statement"":[{{""Action"":[""s3:GetBucketLocation""],""Effect"":""Allow"",""Principal"":{{""AWS"":[""*""]}},""Resource"":[""arn:aws:s3:::{BucketName}""],""Sid"":""""}},{{""Action"":[""s3:ListBucket""],""Condition"":{{""StringEquals"":{{""s3:prefix"":[""foo"",""prefix/""]}}}},""Effect"":""Allow"",""Principal"":{{""AWS"":[""*""]}},""Resource"":[""arn:aws:s3:::{BucketName}""],""Sid"":""""}},{{""Action"":[""s3:GetObject""],""Effect"":""Allow"",""Principal"":{{""AWS"":[""*""]}},""Resource"":[""arn:aws:s3:::{BucketName}/foo*"",""arn:aws:s3:::{BucketName}/prefix/*""],""Sid"":""""}}]}}";

        await _minioClient
            .SetPolicyAsync(
                new SetPolicyArgs()
                    .WithBucket(BucketName)
                    .WithPolicy(policy), cancellationToken);

        _logger.LogInformation("Created bucket {Bucket} and set public policy", BucketName);

    }
}