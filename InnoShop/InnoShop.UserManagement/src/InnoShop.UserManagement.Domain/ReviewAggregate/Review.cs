using ErrorOr;
using InnoShop.UserManagement.Domain.Common;
using InnoShop.UserManagement.Domain.Common.Interfaces;
using InnoShop.UserManagement.Domain.ReviewAggregate.Events;
using InnoShop.UserManagement.Domain.UserAggregate;

namespace InnoShop.UserManagement.Domain.ReviewAggregate;

public sealed class Review : AggregateRoot
{
    public Guid TargetUserId { get; private set; }
    public Guid AuthorId { get; private set; }

    public Rating Rating { get; private set; }
    public Comment? Comment { get; private set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }



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

        _domainEvents.Add(new ReviewCreatedEvent(Id, TargetUserId, Rating.Value));
    }

    public static ErrorOr<Review> Create(
        User targetUser,
        User author,
        Rating rating,
        Comment? comment,
        IDateTimeProvider dateTimeProvider
    )
    {
        if (!targetUser.CanSell() || !author.CanSell())
        {
            return UserErrors.UserMustCreateAUserProfile;
        }

        if (targetUser.Id == author.Id)
        {
            return UserErrors.UserCannotWriteAReviewForThemselves;
        }


        return new Review(
            targetUser.Id,
            author.Id,
            rating,
            comment,
            dateTimeProvider);
    }

    public ErrorOr<Success> Update(
        Rating newRating,
        Comment? newComment,
        IDateTimeProvider dateTimeProvider)
    {
        var oldRating = Rating.Value;
        Rating = newRating;
        Comment = newComment;
        UpdatedAt = dateTimeProvider.UtcNow;

        _domainEvents.Add(new ReviewUpdatedEvent(Id, TargetUserId, NewRating: newRating.Value, OldRating: oldRating));

        return Result.Success;
    }
    public ErrorOr<Deleted> Delete(IDateTimeProvider dateTimeProvider)
    {
        if (IsDeleted)
        {
            return ReviewErrors.ReviewAlreadyDeleted;
        }

        IsDeleted = true;
        DeletedAt = dateTimeProvider.UtcNow;

        _domainEvents.Add(new ReviewDeletedEvent(Id, TargetUserId, Rating.Value));

        return Result.Deleted;
    }

    private Review()
    {
    }
}
