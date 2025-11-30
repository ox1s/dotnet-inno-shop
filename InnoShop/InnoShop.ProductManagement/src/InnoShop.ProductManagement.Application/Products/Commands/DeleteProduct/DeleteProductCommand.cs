using ErrorOr;
using InnoShop.ProductManagement.Application.Common.Security;
using InnoShop.SharedKernel.Security.Permissions;
using MediatR;

namespace InnoShop.ProductManagement.Application.Products.Commands.DeleteProduct;

[Authorize(Permissions = AppPermissions.Product.Delete)]
public record DeleteProductCommand(Guid Id) : IAuthorizeableRequest<ErrorOr<Success>>;