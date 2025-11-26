using ErrorOr;
using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Domain.UserAggregate;
using MediatR;

namespace InnoShop.UserManagement.Application.Users.Commands.DeactivateUser;

public class DeactivateUserProfileCommandHandler(
    IUsersRepository _usersRepository,
    IUnitOfWork _unitOfWork)
    : IRequestHandler<DeactivateUserProfileCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(DeactivateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        var deactivateUserResult = user.DeactivateUser();

        if (deactivateUserResult.IsError)
        {
            return deactivateUserResult.Errors;
        }

        await _usersRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return Result.Success;
    }
}