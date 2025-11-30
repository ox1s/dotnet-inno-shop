using InnoShop.UserManagement.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;

namespace InnoShop.UserManagement.Infrastructure.Security;

public class EmailVerificationLinkFactory(
    IHttpContextAccessor httpContextAccessor,
    LinkGenerator linkGenerator,
    IConfiguration configuration) : IEmailVerificationLinkFactory
{
    public string Create(Guid userId, string token)
    {
        var httpContext = httpContextAccessor.HttpContext;

        string? uri;

        if (httpContext is not null)
        {
            uri = linkGenerator.GetUriByName(
                httpContext,
                "VerifyEmailRoute",
                new { userId, token });
        }
        else
        {
            var appUrl = configuration["AppUrl"];
            if (string.IsNullOrEmpty(appUrl))
                throw new Exception("AppUrl is not configured. Cannot generate email link in background.");

            if (!Uri.TryCreate(appUrl, UriKind.Absolute, out var baseUrl))
                throw new Exception($"Invalid AppUrl configuration: {appUrl}");


            uri = linkGenerator.GetUriByName(
                "VerifyEmailRoute",
                new { userId, token },
                baseUrl.Scheme,
                HostString.FromUriComponent(baseUrl));
        }

        return uri ?? throw new Exception("Could not generate email verification link");
    }

    public string CreateResetPasswordLink(string email, string token)
    {
        var frontendUrl = configuration["WebAppUrl"] ?? "http://localhost:5173";
        
        if (!Uri.TryCreate(frontendUrl, UriKind.Absolute, out var baseUri))
            throw new Exception($"Invalid WebAppUrl configuration: {frontendUrl}");

        var queryParams = new Dictionary<string, string?>
        {
            { "email", email },
            { "token", token }
        };

        return QueryHelpers.AddQueryString($"{baseUri.Scheme}://{baseUri.Authority}/reset-password", queryParams);
    }
}