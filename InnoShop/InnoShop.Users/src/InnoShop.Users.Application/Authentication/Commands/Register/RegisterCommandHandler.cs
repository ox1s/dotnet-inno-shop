using ErrorOr;
using InnoShop.Users.Application.Authentication.Common;
using InnoShop.Users.Application.Common.Interfaces;
using InnoShop.Users.Domain.Common.Interfaces;
using InnoShop.Users.Domain.UserAggregate;
using MediatR;

namespace InnoShop.Users.Application.Authentication.Commands.Register;

public class RegisterCommandHandler(
    IJwtTokenGenerator _jwtTokenGenerator,
    IPasswordHasher _passwordHasher,
    IUsersRepository _usersRepository,
    IUnitOfWork _unitOfWork)
        : IRequestHandler<RegisterCommand, ErrorOr<AuthenticationResult>>
{
    public async Task<ErrorOr<AuthenticationResult>> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var emailResult = Email.Create(command.Email);
        if (emailResult.IsError)
        {
            return emailResult.Errors;
        }

        var email = emailResult.Value;
        if (await _usersRepository.ExistsByEmailAsync(email, cancellationToken))
        {
            return Error.Conflict(description: "User already exists");
        }

        var hashPasswordResult = _passwordHasher.HashPassword(command.Password);
        if (hashPasswordResult.IsError)
        {
            return hashPasswordResult.Errors;
        }

        var user = User.CreateUser(
            email,
            hashPasswordResult.Value);

        await _usersRepository.AddUserAsync(user, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        var token = _jwtTokenGenerator.GenerateToken(user);
        return new AuthenticationResult(user, token);
    }
}