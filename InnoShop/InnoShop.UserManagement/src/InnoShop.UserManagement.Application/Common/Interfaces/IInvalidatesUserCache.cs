namespace InnoShop.UserManagement.Application.Common.Interfaces;

public interface IInvalidatesUserCache
{
    Guid UserId { get; }
}