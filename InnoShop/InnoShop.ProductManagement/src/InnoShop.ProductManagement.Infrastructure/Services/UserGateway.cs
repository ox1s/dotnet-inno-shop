using ErrorOr;
using InnoShop.ProductManagement.Application.Common.Interfaces;
using InnoShop.ProductManagement.Domain.ProductAggregate;

namespace InnoShop.ProductManagement.Infrastructure.Services;

public class UserGateway : IUserGateway
{
    public Task<ErrorOr<SellerSnapshot>> GetSellerSnapshotAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        // TODO: 
        var placeholder = new SellerSnapshot(
            "Placeholder Seller",
            "",
            0.0,
            0);

        return Task.FromResult<ErrorOr<SellerSnapshot>>(placeholder);
    }
}