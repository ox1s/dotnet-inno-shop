using ErrorOr;
using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Domain.UserAggregate;
using MediatR;

namespace InnoShop.UserManagement.Application.Users.Queries.GetUserProfile;

public class GetUserProfileQueryHandler(
    IUsersRepository usersRepository)
    : IRequestHandler<GetUserProfileQuery, ErrorOr<User>>
{
    public async Task<ErrorOr<User>> Handle(GetUserProfileQuery query, CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetByIdAsync(query.UserId, cancellationToken);

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }
        if (!user.CanSell())
        {
            return UserErrors.UserMustCreateAUserProfile;
        }

        return user;
    }
}
