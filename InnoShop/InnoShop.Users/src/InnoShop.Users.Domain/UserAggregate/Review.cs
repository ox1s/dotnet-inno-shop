using ErrorOr;
using InnoShop.Users.Domain.Common;
using InnoShop.Users.Domain.Common.Interfaces;

namespace InnoShop.Users.Domain.UserAggregate;

public class Review : AuditableEntity
{
    public Guid AuthorId { get; private set; }
    public Rating Rating { get; private set; }
    public string Comment { get; private set; }

    internal Review(
        Guid authorId,
        Rating rating,
        string comment,
        IDateTimeProvider dateTimeProvider,
        Guid? id = null) :
         base(id ?? Guid.NewGuid())
    {
        AuthorId = authorId;
        Rating = rating;
        Comment = comment;
        CreatedAt = dateTimeProvider.UtcNow;
        UpdatedAt = dateTimeProvider.UtcNow;
    }

    public static ErrorOr<Review> Create(
        Guid authorId,
        int rawRating,
        string comment,
        IDateTimeProvider dateTimeProvider
    )
    {
        var ratingResult = Rating.Create(rawRating);
        if (ratingResult.IsError)
        {
            return ratingResult.Errors;
        }
        var rating = ratingResult.Value;

        return new Review(
            authorId,
            rating,
            comment,
            dateTimeProvider
        );
    }


    internal void Update(
        Rating rating,
        string comment,
        IDateTimeProvider dateTimeProvider)
    {
        Rating = rating;
        Comment = comment;
        UpdatedAt = dateTimeProvider.UtcNow;
    }

    public Review()
    {
    }
}
