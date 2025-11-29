using ErrorOr;
using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Domain.UserAggregate;
using MediatR;

namespace InnoShop.UserManagement.Application.Authentication.Commands.VerifyEmail;

public class VerifyEmailCommandHandler(
    IUsersRepository usersRepository,
    IUnitOfWork unitOfWork,
    IAuthorizationService authorizationService)
    : IRequestHandler<VerifyEmailCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        var result = user.VerifyEmail(request.Token);

        if (result.IsError)
        {
            return result.Errors;
        }

        await usersRepository.UpdateAsync(user, cancellationToken);
        await unitOfWork.CommitChangesAsync(cancellationToken);

        await authorizationService.InvalidateUserCacheAsync(user.Id);

        return Result.Success;
    }
}