using ErrorOr;
using InnoShop.ProductManagement.Application.Common.Interfaces;
using InnoShop.ProductManagement.Contracts.Products;
using InnoShop.ProductManagement.Domain.ProductAggregate;
using MediatR;

namespace InnoShop.ProductManagement.Application.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler(
    IProductsRepository productsRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserProvider currentUserProvider)
    : IRequestHandler<UpdateProductCommand, ErrorOr<ProductResponse>>
{
    public async Task<ErrorOr<ProductResponse>> Handle(UpdateProductCommand command,
        CancellationToken cancellationToken)
    {
        var product = await productsRepository.GetByIdAsync(command.Id, cancellationToken);

        if (product is null) return ProductErrors.NotFound;

        var currentUser = currentUserProvider.GetCurrentUser();

        if (product.SellerId != currentUser.Id) return ProductErrors.Forbidden;

        var priceResult = Price.Create(command.Price);
        if (priceResult.IsError) return priceResult.Errors;

        var titleResult = Title.Create(command.Title);
        if (titleResult.IsError) return titleResult.Errors;

        var descriptionResult = Description.Create(command.Description);
        if (descriptionResult.IsError) return descriptionResult.Errors;

        var updateResult = product.Update(titleResult.Value, descriptionResult.Value, priceResult.Value);
        if (updateResult.IsError) return updateResult.Errors;

        await productsRepository.UpdateAsync(product, cancellationToken);
        await unitOfWork.CommitChangesAsync(cancellationToken);

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