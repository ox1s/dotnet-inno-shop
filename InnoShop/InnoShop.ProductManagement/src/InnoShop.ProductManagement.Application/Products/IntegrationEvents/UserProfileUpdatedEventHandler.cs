using ErrorOr;
using InnoShop.SharedKernel.IntegrationEvents.UserManagement;
using InnoShop.ProductManagement.Application.Common.Interfaces;
using InnoShop.ProductManagement.Domain.ProductAggregate;
using MediatR;

namespace InnoShop.ProductManagement.Application.Products.IntegrationEvents;

public class UserProfileUpdatedEventHandler(
    IProductsRepository productsRepository,
    IUnitOfWork unitOfWork)
    : INotificationHandler<UserProfileUpdatedIntegrationEvent>
{
    public async Task Handle(UserProfileUpdatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var products = await productsRepository.GetBySellerIdAsync(notification.UserId, cancellationToken, ignoreQueryFilters: true);
        
        var sellerInfo = new SellerSnapshot(
            FullName: $"{notification.FirstName} {notification.LastName}",
            AvatarUrl: notification.AvatarUrl ?? string.Empty,
            Rating: notification.Rating);
        
        foreach (var product in products)
        {
            product.UpdateSellerInfo(sellerInfo);
            await productsRepository.UpdateAsync(product, cancellationToken);
        }
        
        await unitOfWork.CommitChangesAsync(cancellationToken);
    }
}
