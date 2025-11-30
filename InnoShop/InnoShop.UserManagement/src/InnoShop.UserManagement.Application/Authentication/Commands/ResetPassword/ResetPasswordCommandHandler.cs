using ErrorOr;
using InnoShop.SharedKernel.Common;
using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Domain.Common.Interfaces;
using InnoShop.UserManagement.Domain.UserAggregate;
using MediatR;

namespace InnoShop.UserManagement.Application.Authentication.Commands.ResetPassword;

public class ResetPasswordCommandHandler(
    IUsersRepository usersRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher)
    : IRequestHandler<ResetPasswordCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsError) return emailResult.Errors;

        var user = await usersRepository.GetByEmailAsync(emailResult.Value, cancellationToken);
        if (user is null) return UserErrors.UserNotFound;
        
        var result = user.ResetPassword(request.Token, request.NewPassword, passwordHasher);
        if (result.IsError) return result.Errors;

        await usersRepository.UpdateAsync(user, cancellationToken);
        await unitOfWork.CommitChangesAsync(cancellationToken);

        return Result.Success;
    }
}

