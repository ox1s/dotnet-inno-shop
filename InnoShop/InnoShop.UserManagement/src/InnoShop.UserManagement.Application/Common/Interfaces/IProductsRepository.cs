using InnoShop.UserManagement.Domain.UserAggregate;

namespace InnoShop.UserManagement.Application.Common.Interfaces;

public interface IProductsRepository
{
    Task AddProductAsync(User user, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
}