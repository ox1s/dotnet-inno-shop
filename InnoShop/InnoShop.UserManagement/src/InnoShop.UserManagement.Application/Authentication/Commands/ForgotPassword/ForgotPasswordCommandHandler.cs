using ErrorOr;
using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Domain.UserAggregate;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace InnoShop.UserManagement.Application.Authentication.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler(
    IUsersRepository usersRepository,
    IUnitOfWork unitOfWork,
    IDistributedCache cache)
    : IRequestHandler<ForgotPasswordCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsError) return emailResult.Errors;

        var cacheKey = $"reset-password-limit:{request.Email}";
        var attemptsStr = await cache.GetStringAsync(cacheKey, cancellationToken);

        if (!string.IsNullOrEmpty(attemptsStr) && int.Parse(attemptsStr) >= 3)
            return Error.Failure("RateLimit.Exceeded", "Too many requests. Please try again in an hour.");

        var user = await usersRepository.GetByEmailAsync(emailResult.Value, cancellationToken);
        if (user is null) return Result.Success;

        user.RequestPasswordReset();
        await usersRepository.UpdateAsync(user, cancellationToken);
        await unitOfWork.CommitChangesAsync(cancellationToken);

        var newAttempts = string.IsNullOrEmpty(attemptsStr) ? 1 : int.Parse(attemptsStr) + 1;
        await cache.SetStringAsync(
            cacheKey,
            newAttempts.ToString(),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) },
            cancellationToken);

        return Result.Success;
    }
}