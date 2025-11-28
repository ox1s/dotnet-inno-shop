using ErrorOr;
using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Application.Common.Security;

namespace InnoShop.UserManagement.Infrastructure.Security.PolicyEnforcer;

public interface IPolicyEnforcer
{
    public ErrorOr<Success> Authorize<T>(
        IAuthorizeableRequest<T> request,
        CurrentUser currentUser,
        string policy);
}