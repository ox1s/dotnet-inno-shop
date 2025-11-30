using ErrorOr;
using InnoShop.ProductManagement.Application.Common.Interfaces;
using InnoShop.ProductManagement.Domain.ProductAggregate;
using InnoShop.SharedKernel.Security.Roles;
using MediatR;

namespace InnoShop.ProductManagement.Application.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler(
    IProductsRepository productsRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserProvider currentUserProvider)
    : IRequestHandler<DeleteProductCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        var product = await productsRepository.GetByIdAsync(command.Id, cancellationToken, ignoreQueryFilters: true);

        if (product is null)
        {
            return Error.NotFound("Product.NotFound", "Product not found.");
        }

        var currentUser = currentUserProvider.GetCurrentUser();

        // Check if user is the owner or admin
        var isOwner = product.SellerId == currentUser.Id;
        var isAdmin = currentUser.Roles.Contains(AppRoles.Admin);

        if (!isOwner && !isAdmin)
        {
            return Error.Forbidden("Product.Forbidden", "You can only delete your own products or must be an admin.");
        }

        // Soft delete
        product.Hide();

        await productsRepository.UpdateAsync(product, cancellationToken);
        await unitOfWork.CommitChangesAsync(cancellationToken);

        return Result.Success;
    }
}
