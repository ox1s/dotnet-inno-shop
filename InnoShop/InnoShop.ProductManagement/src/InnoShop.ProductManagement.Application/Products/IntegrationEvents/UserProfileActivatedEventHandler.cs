using InnoShop.ProductManagement.Application.Common.Interfaces;
using InnoShop.SharedKernel.IntegrationEvents.UserManagement;
using MediatR;

namespace InnoShop.ProductManagement.Application.Products.IntegrationEvents;

public class UserProfileActivatedEventHandler(
    IProductsRepository productsRepository,
    IUnitOfWork unitOfWork)
    : INotificationHandler<UserProfileActivatedIntegrationEvent>
{
    public async Task Handle(UserProfileActivatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var products = await productsRepository.GetBySellerIdAsync(notification.UserId, cancellationToken, true);
        foreach (var product in products)
        {
            product.Restore();
            await productsRepository.UpdateAsync(product, cancellationToken);
        }

        await unitOfWork.CommitChangesAsync(cancellationToken);
    }
}