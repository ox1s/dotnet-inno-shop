using InnoShop.ProductManagement.Application.Common.Interfaces;
using InnoShop.SharedKernel.IntegrationEvents.UserManagement;
using MediatR;

namespace InnoShop.ProductManagement.Application.Products.IntegrationEvents;

public class UserProfileDeactivatedEventHandler(
    IProductsRepository productsRepository,
    IUnitOfWork unitOfWork)
    : INotificationHandler<UserProfileDeactivatedIntegrationEvent>
{
    public async Task Handle(UserProfileDeactivatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var products = await productsRepository.GetBySellerIdAsync(notification.UserId, cancellationToken, true);
        foreach (var product in products)
        {
            product.Hide();
            await productsRepository.UpdateAsync(product, cancellationToken);
        }

        await unitOfWork.CommitChangesAsync(cancellationToken);
    }
}