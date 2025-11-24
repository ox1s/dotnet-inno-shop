namespace InnoShop.UserManagement.Application.Common.Interfaces;

public interface IUnitOfWork
{
    Task CommitChangesAsync(CancellationToken cancellationToken = default);
}
