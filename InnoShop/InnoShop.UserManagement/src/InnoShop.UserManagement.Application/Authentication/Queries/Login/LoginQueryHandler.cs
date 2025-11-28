using ErrorOr;
using MediatR;

using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Domain.Common.Interfaces;
using InnoShop.UserManagement.Domain.UserAggregate;
using InnoShop.UserManagement.Application.Authentication.Common;

namespace InnoShop.UserManagement.Application.Authentication.Queries.Login;

public class LoginQueryHandler
(
    IJwtTokenGenerator jwtTokenGenerator,
    IPasswordHasher passwordHasher,
    IUsersRepository usersRepository
    ) : IRequestHandler<LoginQuery, ErrorOr<AuthenticationResult>>
{
    public async Task<ErrorOr<AuthenticationResult>> Handle(LoginQuery query, CancellationToken cancellationToken)
    {
        var emailResult = Email.Create(query.Email);
        if (emailResult.IsError)
        {
            return emailResult.Errors;
        }
        var email = emailResult.Value;

        var user = await usersRepository.GetByEmailAsync(email, cancellationToken);

        return user is null || !user.IsCorrectPasswordHash(query.Password, passwordHasher)
                    ? AuthenticationErrors.InvalidCredentials
                    : new AuthenticationResult(user, jwtTokenGenerator.GenerateToken(user));
    }

}
