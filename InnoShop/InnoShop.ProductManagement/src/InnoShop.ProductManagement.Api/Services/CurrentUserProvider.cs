using System.Security.Claims;
using InnoShop.ProductManagement.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
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

    private List<string> GetClaimValues(string claimType) =>
        httpContextAccessor.HttpContext!.User.Claims
            .Where(claim => claim.Type == claimType)
            .Select(claim => claim.Value)
            .ToList();

    private string GetSingleClaimValue(string claimType) =>
        httpContextAccessor.HttpContext!.User.Claims
            .Single(claim => claim.Type == claimType)
            .Value;
}
