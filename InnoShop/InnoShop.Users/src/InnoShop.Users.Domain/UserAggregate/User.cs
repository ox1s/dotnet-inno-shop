using ErrorOr;
using InnoShop.Users.Domain.Common;
using InnoShop.Users.Domain.Common.Interfaces;
using InnoShop.Users.Domain.UserAggregate.Events;

namespace InnoShop.Users.Domain.UserAggregate;

public class User : AggregateRoot
{
    public Email Email { get; private set; } = null!;
    public Role Role { get; private set; } = null!;
    public UserProfile? UserProfile { get; private set; }

    public bool IsActive { get; private set; } = true;
    public bool IsEmailConfirmed { get; private set; } = false;

    private readonly List<Review> _reviews = new();
    public IReadOnlyList<Review> Reviews => _reviews.AsReadOnly();

    private readonly string _passwordHash = null!;


    private User(
        Email email,
        string passwordHash,
        Role role,
        Guid? id = null)
            : base(id ?? Guid.NewGuid())
    {
        Email = email;
        Role = role;
        _passwordHash = passwordHash;
    }

    public bool IsCorrectPasswordHash(string password, IPasswordHasher passwordHasher)
    {
        return passwordHasher.IsCorrectPassword(password, _passwordHash);
    }

    public static User CreateUser(
        Email email,
        string passwordHash)
    {
        return new(email, passwordHash, Role.User);
    }


    public static User CreateAdmin(
        Email email,
        string passwordHash)
    {
        return new(email, passwordHash, Role.Admin);
    }

    public ErrorOr<Success> CreateUserProfile(
       FirstName firstName,
       LastName lastName,
       AvatarUrl avatarUrl,
       string rawPhoneNumber,
       string country,
       string state,
       string? city = null)
    {
        if (UserProfile is not null)
        {
            return UserErrors.CannotCreateMoreThanOneProfile;
        }

        var createdProfileResult = UserProfile.Create(
             firstName, lastName, avatarUrl, rawPhoneNumber,
             country, state, city);


        if (createdProfileResult.IsError)
        {
            return createdProfileResult.Errors;
        }

        UserProfile = createdProfileResult.Value;

        //_domainEvents.Add(new UserProfileUpdatedEvent(Id));

        return Result.Success;
    }

    public ErrorOr<Success> UpdateUserProfile(
            FirstName firstName,
       LastName lastName,
       AvatarUrl avatarUrl,
       string rawPhoneNumber,
       string country,
       string state,
       string? city = null)
    {
        if (UserProfile is null)
        {
            return UserErrors.UserMustCreateAUserProfile;
        }

        var updatedProfileResult = UserProfile.Create(
            firstName, lastName, avatarUrl, rawPhoneNumber,
            country, state, city);

        if (updatedProfileResult.IsError)
        {
            return updatedProfileResult.Errors;
        }

        UserProfile = updatedProfileResult.Value;

        //_domainEvents.Add(new UserProfileCreatedEvent(Id));

        return Result.Success;
    }

    public ErrorOr<Success> CreateReview(
        Guid authorId,
        int rawRating,
        string? comment,
        IDateTimeProvider dateTimeProvider)
    {
        if (UserProfile is null)
        {
            return UserErrors.UserMustCreateAUserProfile;
        }
        if (authorId == Id)
        {
            return UserErrors.UserCannotWriteAReviewForThemselves;
        }
        var createdReviewResult = Review.Create(
            authorId,
            rawRating,
            comment,
            dateTimeProvider);

        if (createdReviewResult.IsError)
        {
            return createdReviewResult.Errors;
        }
        var review = createdReviewResult.Value;

        _reviews.Add(review);
        // _domainEvents.Add(new UserReviewCreatedEvent(Id, review));
        return Result.Success;
    }
    public ErrorOr<Success> UpdateReview(
        Guid reviewId,
        Guid authorId,
        int newRating,
        string? newComment,
        IDateTimeProvider dateTimeProvider)
    {
        var review = _reviews.FirstOrDefault(r => r.Id == reviewId);

        if (review is null)
        {
            return UserErrors.ReviewNotFound;
        }

        if (review.AuthorId != authorId)
        {
            return UserErrors.NotTheReviewAuthor;
        }

        var updatedReviewResult = review.Update(newRating, newComment, dateTimeProvider);
        if (updatedReviewResult.IsError)
        {
            return updatedReviewResult.Errors;
        }

        // _domainEvents.Add(new UserReviewUpdatedEvent(Id, review));

        return Result.Success;
    }

    public ErrorOr<Success> ActivateUser()
    {
        if (IsActive)
        {
            return UserErrors.UserAlreadyActivated;
        }
        IsActive = true;

        // _domainEvents.Add(new UserActivated(Id));
        return Result.Success;
    }
    public ErrorOr<Success> DeactivateUser()
    {
        if (!IsActive)
        {
            return UserErrors.UserAlreadyDeactivated;
        }
        IsActive = false;

        // _domainEvents.Add(new UserDeactivated(Id));
        return Result.Success;
    }
    public bool CanSell()
    {
        return IsActive && UserProfile is not null;
    }
    private User() { }

}
