
using ErrorOr;

namespace InnoShop.UserManagement.Domain.ReviewAggregate;

public static class CommentErrors
{
    public static readonly Error Empty = Error.Validation(
        "Comment.Empty",
        "Comment cannot be empty.");
    public static readonly Error TooLong = Error.Validation(
        "Comment.TooLong",
        "Comment must be under 1000 characters.");
}