using System.Security.Claims;
using InnoShop.ProductManagement.Application.Common.Interfaces;
using Throw;

namespace InnoShop.ProductManagement.Api.Services;

public class CurrentUserProvider(IHttpContextAccessor httpContextAccessor) : ICurrentUserProvider
{
    public CurrentUser GetCurrentUser()
    {
        httpContextAccessor.HttpContext.ThrowIfNull();

        var id = Guid.Parse(GetSingleClaimValue("id"));
        var permissions = GetClaimValues("permissions");
        var roles = GetClaimValues(ClaimTypes.Role);
        var email = GetSingleClaimValue(ClaimTypes.Email);

        return new CurrentUser(id, email, permissions, roles);
    }

    private List<string> GetClaimValues(string claimType)
    {
        return httpContextAccessor.HttpContext!.User.Claims
            .Where(claim => claim.Type == claimType)
            .Select(claim => claim.Value)
            .ToList();
    }

    private string GetSingleClaimValue(string claimType)
    {
        var claim = httpContextAccessor.HttpContext!.User.Claims
            .FirstOrDefault(claim => claim.Type == claimType);
        
        return claim?.Value ?? throw new InvalidOperationException($"Claim '{claimType}' not found in token.");
    }
}