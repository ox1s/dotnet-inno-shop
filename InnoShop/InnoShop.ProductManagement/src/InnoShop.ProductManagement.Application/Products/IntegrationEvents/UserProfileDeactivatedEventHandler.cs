using InnoShop.ProductManagement.Application.Common.Interfaces;
using InnoShop.SharedKernel.IntegrationEvents.UserManagement;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InnoShop.ProductManagement.Application.Products.IntegrationEvents;

public class UserProfileDeactivatedEventHandler(
    IProductsRepository productsRepository,
    IUnitOfWork unitOfWork,
    ILogger<UserProfileDeactivatedEventHandler> logger)
    : INotificationHandler<UserProfileDeactivatedIntegrationEvent>
{
    public async Task Handle(UserProfileDeactivatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Handling UserProfileDeactivatedIntegrationEvent for user {UserId}", notification.UserId);

            var products = await productsRepository.GetBySellerIdAsync(notification.UserId, cancellationToken, true);

            logger.LogInformation("Found {ProductCount} products for seller {UserId}", products.Count, notification.UserId);

            if (products.Count == 0)
            {
                logger.LogInformation("No products found for seller {UserId}. Nothing to deactivate.", notification.UserId);
                return;
            }

            foreach (var product in products)
            {
                try
                {
                    product.Hide();
                    await productsRepository.UpdateAsync(product, cancellationToken);
                    logger.LogInformation("Hid product {ProductId} (Name: {ProductName}) for seller {UserId}",
                        product.Id, product.Title.Value, notification.UserId);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error hiding product {ProductId} for seller {UserId}", product.Id, notification.UserId);
                    throw;
                }
            }

            await unitOfWork.CommitChangesAsync(cancellationToken);

            logger.LogInformation("Successfully hid {ProductCount} products for seller {UserId}", products.Count, notification.UserId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to handle UserProfileDeactivatedIntegrationEvent for user {UserId}. Error: {Error}",
                notification.UserId, ex.Message);
            throw; 
        }
    }
}