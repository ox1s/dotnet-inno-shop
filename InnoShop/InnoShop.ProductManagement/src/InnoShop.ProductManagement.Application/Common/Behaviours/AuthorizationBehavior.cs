using System.Reflection;
using ErrorOr;
using InnoShop.ProductManagement.Application.Common.Interfaces;
using InnoShop.ProductManagement.Application.Common.Security;
using MediatR;

namespace InnoShop.ProductManagement.Application.Common.Behaviours;

public class AuthorizationBehavior<TRequest, TResponse>(
    IAuthorizationService authorizationService)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IAuthorizeableRequest<TResponse>
    where TResponse : IErrorOr
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var authorizationAttributes = request.GetType()
            .GetCustomAttributes<AuthorizeAttribute>()
            .ToList();

        if (authorizationAttributes.Count == 0) return await next(cancellationToken);

        var requiredPermissions = authorizationAttributes
            .SelectMany(authorizationAttribute => authorizationAttribute.Permissions?.Split(',') ?? [])
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => p.Trim())
            .ToList();

        var requiredRoles = authorizationAttributes
            .SelectMany(authorizationAttribute => authorizationAttribute.Roles?.Split(',') ?? [])
            .Where(r => !string.IsNullOrWhiteSpace(r))
            .Select(r => r.Trim())
            .ToList();

        var requiredPolicies = authorizationAttributes
            .SelectMany(authorizationAttribute => authorizationAttribute.Policies?.Split(',') ?? [])
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => p.Trim())
            .ToList();

        var authorizationResult = await authorizationService.AuthorizeCurrentUser(
            request,
            requiredRoles,
            requiredPermissions,
            requiredPolicies);

        return authorizationResult.IsError
            ? (dynamic)authorizationResult.Errors
            : await next(cancellationToken);
    }
}
