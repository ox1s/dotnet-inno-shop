using ErrorOr;

namespace UserManagement.Domain;

public static class UserProfileErrors
{
    public static readonly Error CannotHaveMoreProfilesThanOneForUser = Error.Validation(
        "UserProfile.CannotHaveMoreProfilesThanOneForUser",
        "A user cannot have more than one user profile"
    );
}
