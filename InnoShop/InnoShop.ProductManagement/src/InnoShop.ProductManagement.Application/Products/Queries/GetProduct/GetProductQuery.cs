using ErrorOr;
using InnoShop.ProductManagement.Contracts.Products;
using MediatR;

namespace InnoShop.ProductManagement.Application.Products.Queries.GetProduct;

public record GetProductQuery(Guid Id) : IRequest<ErrorOr<ProductResponse>>;