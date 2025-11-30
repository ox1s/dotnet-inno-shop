using InnoShop.ProductManagement.Application.Common.Interfaces;
using InnoShop.ProductManagement.Domain.ProductAggregate;
using InnoShop.SharedKernel.IntegrationEvents.UserManagement;
using MediatR;

namespace InnoShop.ProductManagement.Application.Products.IntegrationEvents;

public class UserProfileUpdatedEventHandler(
    IProductsRepository productsRepository,
    IUnitOfWork unitOfWork)
    : INotificationHandler<UserProfileUpdatedIntegrationEvent>
{
    public async Task Handle(UserProfileUpdatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var products = await productsRepository.GetBySellerIdAsync(notification.UserId, cancellationToken, true);

        var sellerInfo = new SellerSnapshot(
            $"{notification.FirstName} {notification.LastName}",
            notification.AvatarUrl,
            notification.Rating,
            notification.ReviewCount);

        foreach (var product in products)
        {
            product.UpdateSellerInfo(sellerInfo);
            await productsRepository.UpdateAsync(product, cancellationToken);
        }

        await unitOfWork.CommitChangesAsync(cancellationToken);
    }
}