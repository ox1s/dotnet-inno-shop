namespace InnoShop.ProductManagement.Domain.ProductAggregate;

public sealed record SellerSnapshot(string FullName, string AvatarUrl, double Rating, int ReviewCount);