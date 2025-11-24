using ErrorOr;
using MediatR;

using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Domain.Common.Interfaces;
using InnoShop.UserManagement.Domain.UserAggregate;
using InnoShop.UserManagement.Application.Authentication.Common;

namespace InnoShop.UserManagement.Application.Authentication.Queries.Login;

public class LoginQueryHandler
(
    IJwtTokenGenerator _jwtTokenGenerator,
    IPasswordHasher _passwordHasher,
    IUsersRepository _usersRepository
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

        var user = await _usersRepository.GetByEmailAsync(email, cancellationToken);

        return user is null || !user.IsCorrectPasswordHash(query.Password, _passwordHasher)
                    ? AuthenticationErrors.InvalidCredentials
                    : new AuthenticationResult(user, _jwtTokenGenerator.GenerateToken(user));
    }

}
