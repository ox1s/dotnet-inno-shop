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

        // Get permissions from JWT token claims (they're already in the token)
        // Note: This assumes we're getting permissions for the current user
        // For other users, we'd need to call UserManagement service
        var currentUser = currentUserProvider.GetCurrentUser();
        
        // Only return permissions if the requested user is the current user
        if (currentUser.Id != userId)
        {
            // For now, return empty set if requesting for different user
            // In a real scenario, you'd call UserManagement API to get permissions
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

        // Get roles from JWT token claims (they're already in the token)
        // Note: This assumes we're getting roles for the current user
        // For other users, we'd need to call UserManagement service
        var currentUser = currentUserProvider.GetCurrentUser();
        
        // Only return roles if the requested user is the current user
        if (currentUser.Id != userId)
        {
            // For now, return empty set if requesting for different user
            // In a real scenario, you'd call UserManagement API to get roles
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

        // Policies can be implemented later if needed
        // For now, we'll skip policy enforcement

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
