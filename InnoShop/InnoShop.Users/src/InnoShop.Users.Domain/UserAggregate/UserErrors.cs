using ErrorOr;

namespace InnoShop.Users.Domain.UserAggregate;

public class UserErrors
{
    public static readonly Error UserMustCreateAUserProfile = Error.Conflict(
        "User.UserMustCreateAUserProfile",
        "A User must create a User Profile for this");
    public static readonly Error UserCannotWriteAReviewForThemselves = Error.Conflict(
        "User.UserCannotWriteAReviewForThemselves",
        "A User cannot write a Review for themselves");
    public static readonly Error UserAlreadyActivated = Error.Conflict(
        "User.UserAlreadyActivated",
        "A User already activated");
    public static readonly Error UserAlreadyDeactivated = Error.Conflict(
        "User.UserAlreadyDseactivated",
        "The user is already deactivated");
    public static readonly Error UserProfileMustBeInAllowedCountry = Error.Validation(
        "User.UserProfileMustBeInAllowedCountry",
        "Selling is currently restricted to users located in allowed country.");
    public static readonly Error CannotCreateMoreThanOneProfile = Error.Conflict(
        "User.CannotCreateMoreThanOneProfile",
        "A user can create only one profile.");
    public static readonly Error ReviewNotFound = Error.NotFound(
        "User.ReviewNotFound",
        "The specified review doesn't exist.");
    public static readonly Error NotTheReviewAuthor = Error.Forbidden(
        "User.NotTheReviewAuthor",
        "You are not allowed to edit a review that is not yours.");
}
