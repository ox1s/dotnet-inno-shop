using ErrorOr;
using MediatR;

using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Domain.UserAggregate;

namespace InnoShop.UserManagement.Application.Users.Queries.GetUser;

public class GetUserQueryHandler(
    IUsersRepository _usersRepository)
    : IRequestHandler<GetUserQuery, ErrorOr<User>>
{
    public async Task<ErrorOr<User>> Handle(GetUserQuery query, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByIdAsync(query.UserId, cancellationToken);

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        return user;
    }
}
