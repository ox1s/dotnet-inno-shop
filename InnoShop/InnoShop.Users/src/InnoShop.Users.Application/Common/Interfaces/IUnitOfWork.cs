namespace InnoShop.Users.Application.Common.Interfaces;

public interface IUnitOfWork
{
    Task CommitChangesAsync();
}
