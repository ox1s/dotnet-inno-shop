using ErrorOr;
using InnoShop.Users.Domain.Common;
using InnoShop.Users.Domain.Common.Interfaces;
using InnoShop.Users.Domain.UserAggregate;

namespace InnoShop.Users.Domain.ReviewAggregate;

public sealed class Review : AggregateRoot
{
    public Guid TargetUserId { get; private set; }
    public Guid AuthorId { get; private set; }

    public Rating Rating { get; private set; }
    public Comment? Comment { get; private set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }


    private Review(
        Guid targetUserId,
        Guid authorId,
        Rating rating,
        Comment? comment,
        IDateTimeProvider dateTimeProvider,
        Guid? id = null) :
         base(id ?? Guid.NewGuid())
    {
        TargetUserId = targetUserId;
        AuthorId = authorId;
        Rating = rating;
        Comment = comment;
        var now = dateTimeProvider.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public static ErrorOr<Review> Create(
        Guid targetUserId,
        Guid authorId,
        Rating rating,
        Comment? comment,
        IDateTimeProvider dateTimeProvider
    )
    {
        if (targetUserId == authorId)
        {
            return UserErrors.UserCannotWriteAReviewForThemselves;
        }
        
        // _domainEvents.Add(new ReviewCreatedEvent(review.Id));
        return new Review(
            targetUserId,
            authorId,
            rating,
            comment,
            dateTimeProvider);
    }

    public ErrorOr<Success> Update(
        Rating newRating,
        Comment? newComment,
        IDateTimeProvider dateTimeProvider)
    {
        Rating = newRating;
        Comment = newComment;
        UpdatedAt = dateTimeProvider.UtcNow;
        // _domainEvents.Add(new ReviewUpdatedEvent(review.Id));

        return Result.Success;
    }

    public Review()
    {
    }
}
