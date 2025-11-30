using ErrorOr;
using InnoShop.ProductManagement.Contracts.Products;
using MediatR;

namespace InnoShop.ProductManagement.Application.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string Title,
    string Description,
    decimal Price,
    List<string> ImageUrls) : IRequest<ErrorOr<ProductResponse>>;