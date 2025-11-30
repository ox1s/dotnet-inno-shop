using ErrorOr;
using InnoShop.ProductManagement.Application.Common.Security;
using InnoShop.ProductManagement.Contracts.Products;
using InnoShop.SharedKernel.Security.Permissions;
using MediatR;

namespace InnoShop.ProductManagement.Application.Products.Commands.CreateProduct;

[Authorize(Permissions = AppPermissions.Product.Create)]
public record CreateProductCommand(
    string Title,
    string Description,
    decimal Price,
    List<string> ImageUrls) : IAuthorizeableRequest<ErrorOr<ProductResponse>>;