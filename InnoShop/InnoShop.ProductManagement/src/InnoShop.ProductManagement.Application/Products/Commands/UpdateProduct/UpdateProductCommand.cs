using ErrorOr;
using InnoShop.ProductManagement.Application.Common.Security;
using InnoShop.ProductManagement.Contracts.Products;
using InnoShop.SharedKernel.Security.Permissions;
using MediatR;

namespace InnoShop.ProductManagement.Application.Products.Commands.UpdateProduct;

[Authorize(Permissions = AppPermissions.Product.Update)]
public record UpdateProductCommand(
    Guid Id,
    string Title,
    string Description,
    decimal Price) : IAuthorizeableRequest<ErrorOr<ProductResponse>>;