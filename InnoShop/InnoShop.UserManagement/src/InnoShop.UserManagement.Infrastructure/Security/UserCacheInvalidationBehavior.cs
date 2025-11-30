using ErrorOr;
using InnoShop.UserManagement.Application.Common.Interfaces;
using MediatR;

namespace InnoShop.UserManagement.Infrastructure.Security;

public class UserCacheInvalidationBehavior<TRequest, TResponse>(
    IAuthorizationService authService)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IInvalidatesUserCache
    where TResponse : IErrorOr
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next();

        if (!response.IsError)
        {
            await authService.InvalidateUserCacheAsync(request.UserId);
        }

        return response;
    }
}