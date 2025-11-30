using ErrorOr;

namespace InnoShop.UserManagement.Domain.ReviewAggregate;

public static class RatingErrors
{
    public static readonly Error InvalidRange = Error.Validation(
        "Rating.InvalidRange",
        "Rating can be from one to five"
    );
}