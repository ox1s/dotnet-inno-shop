using ErrorOr;
using MediatR;

using InnoShop.ProductManagement.Contracts.Products;

namespace InnoShop.ProductManagement.Application.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string Title,
    string Description,
    decimal Price,
    List<string> ImageUrls) : IRequest<ErrorOr<ProductResponse>>;
