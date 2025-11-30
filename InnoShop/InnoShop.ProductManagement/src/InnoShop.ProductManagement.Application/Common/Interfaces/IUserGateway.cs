using ErrorOr;
using InnoShop.ProductManagement.Domain.ProductAggregate;

namespace InnoShop.ProductManagement.Application.Common.Interfaces;

public interface IUserGateway
{
    Task<ErrorOr<SellerSnapshot>> GetSellerSnapshotAsync(Guid userId, CancellationToken cancellationToken = default);
}
