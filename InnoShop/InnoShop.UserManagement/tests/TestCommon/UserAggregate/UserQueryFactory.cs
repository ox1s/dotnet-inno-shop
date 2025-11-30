using InnoShop.UserManagement.Application.Users.Queries.GetUserProfile;

namespace InnoShop.UserManagement.TestCommon.UserAggregate;

public static class UserQueryFactory
{
    public static GetUserProfileQuery CreateGetUserProfileQuery(Guid userId)
    {
        return new GetUserProfileQuery(userId);
    }
}