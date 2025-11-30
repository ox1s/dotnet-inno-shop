using ErrorOr;
using InnoShop.ProductManagement.Application.Common.Interfaces;
using InnoShop.ProductManagement.Contracts.Products;
using InnoShop.ProductManagement.Domain.ProductAggregate;
using MediatR;

namespace InnoShop.ProductManagement.Application.Products.Commands.CreateProduct;

public class CreateProductCommandHandler(
    IProductsRepository productsRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserProvider currentUserProvider,
    IUserGateway userGateway)
    : IRequestHandler<CreateProductCommand, ErrorOr<ProductResponse>>
{
    public async Task<ErrorOr<ProductResponse>> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var currentUser = currentUserProvider.GetCurrentUser();
        var sellerId = currentUser.Id;

        var sellerSnapshotResult = await userGateway.GetSellerSnapshotAsync(sellerId, cancellationToken);
        if (sellerSnapshotResult.IsError) return sellerSnapshotResult.Errors;
    
        var priceResult = Price.Create(command.Price);
        if (priceResult.IsError) return priceResult.Errors;

        var images = new List<Image>();
        foreach (var imageUrl in command.ImageUrls)
        {
            var imageResult = Image.Create(imageUrl);
            if (imageResult.IsError) return imageResult.Errors;
            images.Add(imageResult.Value);
        }

        var product = Product.CreateProduct(
            id: Guid.NewGuid(),
            title: command.Title,
            description: command.Description,
            price: priceResult.Value,
            sellerId: sellerId,
            sellerInfo: sellerSnapshotResult.Value,
            images: images);

        await productsRepository.AddAsync(product, cancellationToken);
        await unitOfWork.CommitChangesAsync(cancellationToken);

        return new ProductResponse(
            product.Id,
            product.Title,
            product.Description,
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
