using ErrorOr;
using InnoShop.UserManagement.Application.Common.Security;

namespace InnoShop.UserManagement.Application.Common.Interfaces;

public interface IAuthorizationService
{
    Task<ErrorOr<Success>> AuthorizeCurrentUser<T>(
        IAuthorizeableRequest<T> request,
        List<string> requiredRoles,
        List<string> requiredPermissions,
        List<string> requiredPolicies);

    Task<HashSet<string>> GetPermissionsForUserAsync(Guid identityId);
    Task<HashSet<string>> GetRolesForUserAsync(Guid identityId);
}