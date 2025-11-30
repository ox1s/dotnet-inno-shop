using ErrorOr;
using InnoShop.ProductManagement.Contracts.Products;
using MediatR;

namespace InnoShop.ProductManagement.Application.Products.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
    string Title,
    string Description,
    decimal Price) : IRequest<ErrorOr<ProductResponse>>;