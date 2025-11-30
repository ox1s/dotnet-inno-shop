namespace InnoShop.ProductManagement.Contracts.Products;

public record UpdateProductRequest(
    string Title,
    string Description,
    decimal Price);