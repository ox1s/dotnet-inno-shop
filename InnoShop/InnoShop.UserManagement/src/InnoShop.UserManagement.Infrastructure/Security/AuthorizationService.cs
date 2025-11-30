using System.Text.Json;
using ErrorOr;
using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Application.Common.Security;
using InnoShop.UserManagement.Infrastructure.Persistence;
using InnoShop.UserManagement.Infrastructure.Security.PolicyEnforcer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace InnoShop.UserManagement.Infrastructure.Security;

public class AuthorizationService(
    UserManagementDbContext dbContext,
    IDistributedCache cache,
    IPolicyEnforcer policyEnforcer,
    ICurrentUserProvider currentUserProvider)
    : IAuthorizationService
{
    public async Task<HashSet<string>> GetPermissionsForUserAsync(Guid userId)
    {
        var cacheKey = $"auth:permissions-{userId}";

        var cachedPermissions = await cache.GetStringAsync(cacheKey);
        if (cachedPermissions is not null) return JsonSerializer.Deserialize<HashSet<string>>(cachedPermissions)!;


        var permissions = await dbContext.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .SelectMany(u => u.Roles)
            .SelectMany(r => r.Permissions)
            .Select(p => p.Name)
            .ToListAsync();

        var permissionsSet = permissions.ToHashSet();

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

        var roles = await dbContext.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .SelectMany(u => u.Roles)
            .Select(r => r.Name)
            .ToListAsync();

        var rolesSet = roles.ToHashSet();

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

        if (requiredPermissions.Except(userPermissions).Any())
            return Error.Unauthorized(description: "User is missing required permissions.");

        if (requiredRoles.Except(userRoles).Any())
            return Error.Forbidden(description: "User is missing required roles.");

        foreach (var policy in requiredPolicies)
        {
            var result = policyEnforcer.Authorize(request, currentUser, policy);
            if (result.IsError) return result.Errors;
        }

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