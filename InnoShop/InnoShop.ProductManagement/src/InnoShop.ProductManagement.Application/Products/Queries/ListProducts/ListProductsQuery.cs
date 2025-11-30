using ErrorOr;
using InnoShop.ProductManagement.Contracts.Products;
using MediatR;

namespace InnoShop.ProductManagement.Application.Products.Queries.ListProducts;

public record ListProductsQuery(
    int Page = 1,
    int PageSize = 10) : IRequest<ErrorOr<PagedProductResponse>>;
