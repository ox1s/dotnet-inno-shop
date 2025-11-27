using ErrorOr;
using InnoShop.UserManagement.Application.Common.Security.Request;

namespace InnoShop.UserManagement.Application.Common.Interfaces;

public interface IAuthorizationService
{
    ErrorOr<Success> AuthorizeCurrentUser<T>(
        IAuthorizeableRequest<T> request,
        List<string> requiredRoles,
        List<string> requiredPermissions,
        List<string> requiredPolicies);
}