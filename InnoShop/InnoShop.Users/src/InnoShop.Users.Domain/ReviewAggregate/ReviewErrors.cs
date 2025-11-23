using ErrorOr;

namespace InnoShop.Users.Domain.ReviewAggregate;

public static class ReviewErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "Review.ReviewNotFound",
        "Review not found");
    public static readonly Error ReviewAlreadyDeleted = Error.Conflict(
    "Review.ReviewAlreadyDeleted",
    "The review is alredy deleted");
}
