using ErrorOr;
using InnoShop.ProductManagement.Application.Common.Interfaces;
using InnoShop.ProductManagement.Contracts.Products;
using MediatR;

namespace InnoShop.ProductManagement.Application.Products.Queries.ListProducts;

public class ListProductsQueryHandler(IProductsRepository productsRepository)
    : IRequestHandler<ListProductsQuery, ErrorOr<PagedProductResponse>>
{
    public async Task<ErrorOr<PagedProductResponse>> Handle(ListProductsQuery query, CancellationToken cancellationToken)
    {
        var (products, totalCount) = await productsRepository.GetPagedAsync(query.Page, query.PageSize, cancellationToken);

        var productResponses = products.Select(p => new ProductResponse(
            p.Id,
            p.Title,
            p.Description,
            p.Price.Value,
            p.SellerId,
            new SellerInfoResponse(
                p.SellerInfo.FullName,
                p.SellerInfo.AvatarUrl,
                p.SellerInfo.Rating),
            p.Images.Select(i => i.Url).ToList(),
            p.CreatedAt,
            p.UpdatedAt,
            p.IsActive)).ToList();

        var totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);

        return new PagedProductResponse(
            productResponses,
            query.Page,
            query.PageSize,
            totalCount,
            totalPages);
    }
}
