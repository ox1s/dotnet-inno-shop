using MediatR;

namespace InnoShop.UserManagement.Application.Common.Security;
public interface IAuthorizeableRequest<T> : IRequest<T>
{
    Guid UserId { get; }
}