namespace InnoShop.ProductManagement.Contracts.Products;

public record CreateProductRequest(
    string Title,
    string Description,
    decimal Price,
    List<string> ImageUrls);