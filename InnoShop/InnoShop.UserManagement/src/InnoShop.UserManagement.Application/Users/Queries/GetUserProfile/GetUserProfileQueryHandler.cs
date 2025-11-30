using ErrorOr;
using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Contracts.Users;
using InnoShop.UserManagement.Domain.UserAggregate;
using MediatR;

namespace InnoShop.UserManagement.Application.Users.Queries.GetUserProfile;

public class GetUserProfileQueryHandler(
    IUsersRepository usersRepository)
    : IRequestHandler<GetUserProfileQuery, ErrorOr<UserProfileResponse>>
{
    public async Task<ErrorOr<UserProfileResponse>> Handle(GetUserProfileQuery query,
        CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetByIdAsync(query.UserId, cancellationToken);

        if (user is null) return UserErrors.UserNotFound;
        if (!user.CanSell()) return UserErrors.UserMustCreateAUserProfile;

        var profile = user.UserProfile!;
        return new UserProfileResponse(
            user.Id,
            profile.FirstName.Value,
            profile.LastName.Value,
            profile.AvatarUrl.Value,
            profile.PhoneNumber.Value,
            profile.Location.Country.Name,
            profile.Location.City);
    }
}