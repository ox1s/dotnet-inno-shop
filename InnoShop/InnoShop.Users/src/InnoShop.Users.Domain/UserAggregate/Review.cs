using ErrorOr;
using InnoShop.Users.Domain.Common;
using InnoShop.Users.Domain.Common.Interfaces;

namespace InnoShop.Users.Domain.UserAggregate;

public sealed class Review : AuditableEntity
{
    public Guid AuthorId { get; private set; }
    public Rating Rating { get; private set; } = null!;
    public Comment? Comment { get; private set; }

    internal Review(
        Guid authorId,
        Rating rating,
        Comment? comment,
        IDateTimeProvider dateTimeProvider,
        Guid? id = null) :
         base(id ?? Guid.NewGuid())
    {
        AuthorId = authorId;
        Rating = rating;
        Comment = comment;
        var now = dateTimeProvider.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public static ErrorOr<Review> Create(
        Guid authorId,
        int rawRating,
        string? rawComment,
        IDateTimeProvider dateTimeProvider
    )
    {
        var ratingResult = Rating.Create(rawRating);
        if (ratingResult.IsError)
        {
            return ratingResult.Errors;
        }
        var rating = ratingResult.Value;

        Comment? comment = null;
        if (!string.IsNullOrWhiteSpace(rawComment))
        {
            var commentResult = Comment.Create(rawComment);
            if (commentResult.IsError)
            {
                return commentResult.Errors;
            }
            comment = commentResult.Value;
        }

        return new Review(
            authorId,
            rating,
            comment,
            dateTimeProvider);
    }

    public ErrorOr<Success> Update(
        int newRawRating,
        string? newRawComment,
        IDateTimeProvider dateTimeProvider)
    {
        var ratingResult = Rating.Create(newRawRating);
        if (ratingResult.IsError)
        {
            return ratingResult.Errors;
        }

        Comment? comment = null;
        if (!string.IsNullOrWhiteSpace(newRawComment))
        {
            var commentResult = Comment.Create(newRawComment);
            if (commentResult.IsError)
            {
                return commentResult.Errors;
            }
            comment = commentResult.Value;
        }

        Rating = ratingResult.Value;
        Comment = comment;
        UpdatedAt = dateTimeProvider.UtcNow;
        return Result.Success;
    }

    public Review()
    {
    }
}
