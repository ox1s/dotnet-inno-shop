namespace InnoShop.ProductManagement.Contracts.Products;

public record ProductResponse(
    Guid Id,
    string Title,
    string Description,
    decimal Price,
    Guid SellerId,
    SellerInfoResponse SellerInfo,
    List<string> ImageUrls,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    bool IsActive);
