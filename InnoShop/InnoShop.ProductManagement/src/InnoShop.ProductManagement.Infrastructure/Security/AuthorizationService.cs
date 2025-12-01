using System.Text.Json;
using ErrorOr;
using InnoShop.ProductManagement.Application.Common.Interfaces;
using InnoShop.ProductManagement.Application.Common.Security;
using Microsoft.Extensions.Caching.Distributed;

namespace InnoShop.ProductManagement.Infrastructure.Security;

public class AuthorizationService(
    IDistributedCache cache,
    ICurrentUserProvider currentUserProvider)
    : IAuthorizationService
{
    public async Task<HashSet<string>> GetPermissionsForUserAsync(Guid userId)
    {
        var cacheKey = $"auth:permissions-{userId}";

        var cachedPermissions = await cache.GetStringAsync(cacheKey);
        if (cachedPermissions is not null) return JsonSerializer.Deserialize<HashSet<string>>(cachedPermissions)!;

        var currentUser = currentUserProvider.GetCurrentUser();

        if (currentUser.Id != userId)
        {
            return new HashSet<string>();
        }

        var permissionsSet = currentUser.Permissions.ToHashSet();

        await cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(permissionsSet),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) });

        return permissionsSet;
    }

    public async Task<HashSet<string>> GetRolesForUserAsync(Guid userId)
    {
        var cacheKey = $"auth:roles-{userId}";

        var cachedRoles = await cache.GetStringAsync(cacheKey);
        if (cachedRoles is not null) return JsonSerializer.Deserialize<HashSet<string>>(cachedRoles)!;

        var currentUser = currentUserProvider.GetCurrentUser();

        if (currentUser.Id != userId)
        {
            return new HashSet<string>();
        }

        var rolesSet = currentUser.Roles.ToHashSet();

        await cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(rolesSet),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) });

        return rolesSet;
    }

    public async Task<ErrorOr<Success>> AuthorizeCurrentUser<T>(
        IAuthorizeableRequest<T> request,
        List<string> requiredRoles,
        List<string> requiredPermissions,
        List<string> requiredPolicies)
    {
        var currentUser = currentUserProvider.GetCurrentUser();

        var userPermissions = await GetPermissionsForUserAsync(currentUser.Id);
        var userRoles = await GetRolesForUserAsync(currentUser.Id);

        if (requiredPermissions.Any() && requiredPermissions.Except(userPermissions).Any())
            return Error.Unauthorized(description: "User is missing required permissions.");

        if (requiredRoles.Any() && requiredRoles.Except(userRoles).Any())
            return Error.Forbidden(description: "User is missing required roles.");

        return Result.Success;
    }

    public async Task InvalidateUserCacheAsync(Guid userId)
    {
        var permissionsCacheKey = $"auth:permissions-{userId}";
        var rolesCacheKey = $"auth:roles-{userId}";

        await cache.RemoveAsync(permissionsCacheKey);
        await cache.RemoveAsync(rolesCacheKey);
    }
}
