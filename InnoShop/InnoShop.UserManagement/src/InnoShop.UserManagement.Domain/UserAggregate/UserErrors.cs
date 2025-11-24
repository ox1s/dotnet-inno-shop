using ErrorOr;

namespace InnoShop.UserManagement.Domain.UserAggregate;

public class UserErrors
{
    public static readonly Error UserNotFound = Error.NotFound(
        "User.UserNotFound",
        "User not found");
    public static readonly Error RatingMismatch = Error.Unexpected(
        "User.RatingMismatch",
        "Cannot remove rating because review count is already zero.");
    public static readonly Error UserCannotReviewTwice = Error.Conflict(
        "User.UserCannotReviewTwice",
        "A User cannot add review to one user profile twice");
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
    public static readonly Error NotFoundByEmail = Error.NotFound(
        "Users.NotFoundByEmail",
        "The user with the specified email was not found");

    public static readonly Error UserAlreadyExist = Error.Conflict(
        "Users.UserAlreadyExist",
        "User with this email already exist");
}
