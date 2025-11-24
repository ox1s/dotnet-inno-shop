using System.Reflection;
using ErrorOr;
using MediatR;
using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Application.Common.Authorization;

namespace InnoShop.UserManagement.Application.Common.Behaviours;

public class AuthorizationBehavior<TRequest, TResponse>(ICurrentUserProvider _currentUserProvider)
    : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : IErrorOr
{
    // Перехват
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Смотрит на атрибуты типа authorize
        var authorizationAttributes = request.GetType()
            .GetCustomAttributes<AuthorizeAttribute>()
            .ToList();

        // Если няма идет дальше
        if (authorizationAttributes.Count == 0)
        {
            return await next();
        }

        // Если есть, то получает текущего юзера и проверяет его атрибуты, у меня тут
        // права на узера и роли его
        var currentUser = _currentUserProvider.GetCurrentUser();

        var requiredPermissions = authorizationAttributes
            .SelectMany(authorizationAttribute => authorizationAttribute.Permissions?.Split(',') ?? [])
            .ToList();

        if (requiredPermissions.Except(currentUser.Permissions).Any())
        {
            return (dynamic)Error.Unauthorized(description: "User is forbidden from taking this action");
        }

        var requiredRoles = authorizationAttributes
            .SelectMany(authorizationAttribute => authorizationAttribute.Roles?.Split(',') ?? [])
            .ToList();

        if (requiredRoles.Except(currentUser.Roles).Any())
        {
            return (dynamic)Error.Unauthorized(description: "User is forbidden from taking this action");
        }

        return await next();
    }
}
