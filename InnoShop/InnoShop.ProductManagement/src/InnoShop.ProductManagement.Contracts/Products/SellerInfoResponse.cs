namespace InnoShop.ProductManagement.Contracts.Products;

public record SellerInfoResponse(
    string FullName,
    string AvatarUrl,
    double Rating);