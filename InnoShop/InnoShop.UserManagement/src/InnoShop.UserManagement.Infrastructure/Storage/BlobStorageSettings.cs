namespace InnoShop.UserManagement.Infrastructure.Storage;

public class BlobStorageSettings
{
    public const string Section = "BlobStorage";

    public string BucketName { get; init; } = "user-avatars";
}