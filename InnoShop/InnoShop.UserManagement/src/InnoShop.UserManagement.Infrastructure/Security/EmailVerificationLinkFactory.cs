using InnoShop.UserManagement.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
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
            {
                throw new Exception("AppUrl is not configured. Cannot generate email link in background.");
            }

            if (!Uri.TryCreate(appUrl, UriKind.Absolute, out var baseUrl))
            {
                throw new Exception($"Invalid AppUrl configuration: {appUrl}");
            }


            uri = linkGenerator.GetUriByName(
                "VerifyEmailRoute",
                new { userId, token },
                scheme: baseUrl.Scheme,
                host: HostString.FromUriComponent(baseUrl));
        }

        return uri ?? throw new Exception("Could not generate email verification link");
    }

    public string CreateResetPasswordLink(string email, string token)
    {
        // TODO: Сделать не через контентинацию
        var frontendUrl = configuration["WebAppUrl"] ?? "http://localhost:5173";
        return $"{frontendUrl}/reset-password?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";
    }
}