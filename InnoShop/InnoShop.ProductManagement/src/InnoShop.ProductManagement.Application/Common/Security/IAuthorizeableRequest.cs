using MediatR;

namespace InnoShop.ProductManagement.Application.Common.Security;

public interface IAuthorizeableRequest<T> : IRequest<T>
{
}
