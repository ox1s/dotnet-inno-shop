using ErrorOr;
using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Domain.UserAggregate;
using MediatR;

namespace InnoShop.UserManagement.Application.Users.Commands.ActivateUser;

public class ActivateUserProfileCommandHandler(
    IUsersRepository _usersRepository,
    IUnitOfWork _unitOfWork) 
    : IRequestHandler<ActivateUserProfileCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(ActivateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        var activateUserResult = user.ActivateUser();

        if (activateUserResult.IsError)
        {
            return activateUserResult.Errors;
        }

        await _usersRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return Result.Success;
    }
}