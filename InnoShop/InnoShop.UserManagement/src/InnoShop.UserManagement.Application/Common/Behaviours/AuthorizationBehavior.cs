using ErrorOr;
using InnoShop.UserManagement.Application.Common.Interfaces;
using MediatR;
using System.Reflection;
using InnoShop.UserManagement.Application.Common.Security;

namespace InnoShop.UserManagement.Application.Common.Behaviours;

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

        if (authorizationAttributes.Count == 0)
        {
            return await next(cancellationToken);
        }

        var requiredPermissions = authorizationAttributes
            .SelectMany(authorizationAttribute => authorizationAttribute.Permissions?.Split(',') ?? [])
            .ToList();

        var requiredRoles = authorizationAttributes
            .SelectMany(authorizationAttribute => authorizationAttribute.Roles?.Split(',') ?? [])
            .ToList();

        var requiredPolicies = authorizationAttributes
            .SelectMany(authorizationAttribute => authorizationAttribute.Policies?.Split(',') ?? [])
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