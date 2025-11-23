using InnoShop.Users.Domain.UserAggregate;

namespace InnoShop.Users.Application.Common.Interfaces;

public interface IUsersRepository
{
    Task AddUserAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
}