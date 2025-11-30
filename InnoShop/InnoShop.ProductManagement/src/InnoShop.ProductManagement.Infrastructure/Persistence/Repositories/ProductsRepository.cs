using InnoShop.ProductManagement.Application.Common.Interfaces;
using InnoShop.ProductManagement.Domain.ProductAggregate;
using Microsoft.EntityFrameworkCore;

namespace InnoShop.ProductManagement.Infrastructure.Persistence.Repositories;

public class ProductsRepository(ProductManagementDbContext dbContext) : IProductsRepository
{
    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken, bool ignoreQueryFilters = false)
    {
        var query = dbContext.Products.AsQueryable();
        if (ignoreQueryFilters) query = query.IgnoreQueryFilters();
        return await query.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<List<Product>> GetBySellerIdAsync(Guid sellerId, CancellationToken cancellationToken, bool ignoreQueryFilters = false)
    {
        var query = dbContext.Products.AsQueryable();
        if (ignoreQueryFilters) query = query.IgnoreQueryFilters();
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

    public async Task<(List<Product> Products, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Products.AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);

        var products = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (products, totalCount);
    }
}
