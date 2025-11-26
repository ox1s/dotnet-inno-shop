using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Domain.UserAggregate;

namespace InnoShop.UserManagement.Infrastructure.Persistence.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly UserManagementDbContext _dbContext;

    public UsersRepository(UserManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task AddUserAsync(User user, CancellationToken cancellationToken = default)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
    }

    public Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.FindAsync(userId, cancellationToken);
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

}
