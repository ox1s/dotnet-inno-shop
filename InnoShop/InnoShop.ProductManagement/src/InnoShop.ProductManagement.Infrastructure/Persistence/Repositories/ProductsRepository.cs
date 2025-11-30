using InnoShop.ProductManagement.Application.Common.Interfaces;
using InnoShop.ProductManagement.Domain.ProductAggregate;
using InnoShop.ProductManagement.Infrastructure.Persistence.Filters;
using Microsoft.EntityFrameworkCore;

namespace InnoShop.ProductManagement.Infrastructure.Persistence.Repositories;

public class ProductsRepository(ProductManagementDbContext dbContext) : IProductsRepository
{
    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken,
        bool ignoreQueryFilters = false)
    {
        var query = dbContext.Products.AsQueryable();
        if (ignoreQueryFilters) query = query.IgnoreQueryFilters([ProductsFilters.ActiveProducts]);
        return await query.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<List<Product>> GetBySellerIdAsync(Guid sellerId, CancellationToken cancellationToken,
        bool ignoreQueryFilters = false)
    {
        var query = dbContext.Products.AsQueryable();
        if (ignoreQueryFilters) query = query.IgnoreQueryFilters([ProductsFilters.ActiveProducts]);
        return await query.Where(p => p.SellerId == sellerId).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        await dbContext.Products.AddAsync(product, cancellationToken);
    }

    public Task UpdateAsync(Product product, CancellationToken cancellationToken)
    {
        dbContext.Products.Update(product);
        return Task.CompletedTask;
    }

    public async Task<(List<Product> Products, int TotalCount)> GetPagedAsync(
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
        CancellationToken cancellationToken = default)
    {
        var query = isAvailable.HasValue && !isAvailable.Value
            ? dbContext.Products.IgnoreQueryFilters([ProductsFilters.ActiveProducts]).AsQueryable()
            : dbContext.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLower();

            query = query.Where(p =>
                p.Title.Value.Contains(lowerSearchTerm, StringComparison.CurrentCultureIgnoreCase) ||
                p.Description.Value.Contains(lowerSearchTerm, StringComparison.CurrentCultureIgnoreCase));
        }

        if (minPrice.HasValue) query = query.Where(p => p.Price.Value >= minPrice.Value);

        if (maxPrice.HasValue) query = query.Where(p => p.Price.Value <= maxPrice.Value);

        if (sellerId.HasValue) query = query.Where(p => p.SellerId == sellerId.Value);

        if (categoryId.HasValue) query = query.Where(p => p.CategoryId == categoryId.Value);

        if (isAvailable.HasValue && !isAvailable.Value)
            query = query.Where(p => !p.IsActive);
        else if (isAvailable.HasValue && isAvailable.Value) query = query.Where(p => p.IsActive);

        var totalCount = await query.CountAsync(cancellationToken);

        var isDescending = sortOrder?.ToLower() == "desc";
        query = sortBy?.ToLower() switch
        {
            "title" => isDescending
                ? query.OrderByDescending(p => p.Title)
                : query.OrderBy(p => p.Title),
            "price" => isDescending
                ? query.OrderByDescending(p => p.Price.Value)
                : query.OrderBy(p => p.Price.Value),
            "updatedat" => isDescending
                ? query.OrderByDescending(p => p.UpdatedAt)
                : query.OrderBy(p => p.UpdatedAt),
            "createdat" or _ => isDescending
                ? query.OrderByDescending(p => p.CreatedAt)
                : query.OrderBy(p => p.CreatedAt)
        };

        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (products, totalCount);
    }
}