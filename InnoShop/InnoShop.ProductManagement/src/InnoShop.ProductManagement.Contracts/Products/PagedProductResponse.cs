namespace InnoShop.ProductManagement.Contracts.Products;

public record PagedProductResponse(
    List<ProductResponse> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
