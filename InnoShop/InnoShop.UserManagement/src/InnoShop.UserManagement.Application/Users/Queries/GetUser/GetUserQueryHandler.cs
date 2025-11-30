using ErrorOr;
using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Contracts.Users;
using InnoShop.UserManagement.Domain.UserAggregate;
using MediatR;

namespace InnoShop.UserManagement.Application.Users.Queries.GetUser;

public class GetUserQueryHandler(
    IUsersRepository usersRepository)
    : IRequestHandler<GetUserQuery, ErrorOr<UserResponse>>
{
    public async Task<ErrorOr<UserResponse>> Handle(GetUserQuery query, CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetByIdAsync(query.UserId, cancellationToken);

        if (user is null) return UserErrors.UserNotFound;

        return new UserResponse(
            user.Id,
            user.Email.Value,
            user.Roles.Select(r => r.Name).ToList(),
            user.IsEmailVerified,
            user.IsActive,
            user.UserProfile != null
                ? new UserProfileResponse(
                    user.Id,
                    user.UserProfile.FirstName.Value,
                    user.UserProfile.LastName.Value,
                    user.UserProfile.AvatarUrl.Value,
                    user.UserProfile.PhoneNumber.Value,
                    user.UserProfile.Location.Country.Name,
                    user.UserProfile.Location.City)
                : null);
    }
}