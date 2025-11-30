using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Domain.UserAggregate;
using Microsoft.EntityFrameworkCore;

namespace InnoShop.UserManagement.Infrastructure.Persistence.Repositories;

public class UsersRepository(UserManagementDbContext dbContext) : IUsersRepository
{
    public async Task AddUserAsync(User user, CancellationToken cancellationToken = default)
    {
        foreach (var role in user.Roles) dbContext.Attach(role);
        await dbContext.Users.AddAsync(user, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .AnyAsync(x => x.Email == email, cancellationToken);
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .AnyAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .Include(u => u.Roles)
            .ThenInclude(r => r.Permissions)
            .Include(u => u.UserProfile)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        dbContext.Users.Update(user);

        return Task.CompletedTask;
    }
}