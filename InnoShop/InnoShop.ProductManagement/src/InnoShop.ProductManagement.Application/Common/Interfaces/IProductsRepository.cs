using InnoShop.ProductManagement.Domain.ProductAggregate;

namespace InnoShop.ProductManagement.Application.Common.Interfaces;

public interface IProductsRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default,
        bool ignoreQueryFilters = false);

    Task<List<Product>> GetBySellerIdAsync(Guid sellerId, CancellationToken cancellationToken = default,
        bool ignoreQueryFilters = false);

    Task<(List<Product> Products, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        Guid? sellerId = null,
        Guid? categoryId = null,
        bool? isAvailable = null,
        string? sortBy = null,
        string? sortOrder = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    Task UpdateAsync(Product product, CancellationToken cancellationToken = default);
}