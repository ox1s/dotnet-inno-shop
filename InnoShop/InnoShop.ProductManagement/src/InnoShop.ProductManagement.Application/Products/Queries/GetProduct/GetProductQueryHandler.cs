using ErrorOr;
using InnoShop.ProductManagement.Application.Common.Interfaces;
using InnoShop.ProductManagement.Contracts.Products;
using InnoShop.ProductManagement.Domain.ProductAggregate;
using MediatR;

namespace InnoShop.ProductManagement.Application.Products.Queries.GetProduct;

public class GetProductQueryHandler(IProductsRepository productsRepository)
    : IRequestHandler<GetProductQuery, ErrorOr<ProductResponse>>
{
    public async Task<ErrorOr<ProductResponse>> Handle(GetProductQuery query, CancellationToken cancellationToken)
    {
        var product = await productsRepository.GetByIdAsync(query.Id, cancellationToken);

        if (product is null) return ProductErrors.NotFound;

        return new ProductResponse(
            product.Id,
            product.Title.Value,
            product.Description.Value,
            product.Price.Value,
            product.SellerId,
            new SellerInfoResponse(
                product.SellerInfo.FullName,
                product.SellerInfo.AvatarUrl,
                product.SellerInfo.Rating),
            product.Images.Select(i => i.Url).ToList(),
            product.CreatedAt,
            product.UpdatedAt,
            product.IsActive);
    }
}