using ErrorOr;
using InnoShop.ProductManagement.Application.Common.Security;

namespace InnoShop.ProductManagement.Application.Common.Interfaces;

public interface IAuthorizationService
{
    Task<ErrorOr<Success>> AuthorizeCurrentUser<T>(
        IAuthorizeableRequest<T> request,
        List<string> requiredRoles,
        List<string> requiredPermissions,
        List<string> requiredPolicies);

    Task<HashSet<string>> GetPermissionsForUserAsync(Guid userId);
    Task<HashSet<string>> GetRolesForUserAsync(Guid userId);
    Task InvalidateUserCacheAsync(Guid userId);
}
