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
    public double? AverageRating { get; private set; }
    public int? ReviewCount { get; private set; }


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

    public static User CreateUser(Email email, string passwordHash)
    {
        return new(email, passwordHash, Role.User);
    }

    public static User CreateAdmin(Email email, string passwordHash)
    {
        return new(email, passwordHash, Role.Admin);
    }

    public ErrorOr<Success> CreateUserProfile(UserProfile userProfile)
    {
        if (UserProfile is not null)
        {
            return UserErrors.CannotCreateMoreThanOneProfile;
        }

        UserProfile = userProfile;

        //_domainEvents.Add(new UserProfileUpdatedEvent(Id));

        return Result.Success;
    }

    public ErrorOr<Success> UpdateUserProfile(UserProfile updatedUserProfile)
    {
        if (UserProfile is null)
        {
            return UserErrors.UserMustCreateAUserProfile;
        }

        UserProfile = updatedUserProfile;

        //_domainEvents.Add(new UserProfileCreatedEvent(Id));

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

        // _domainEvents.Add(new UserDeactivatedEvent(Id));
        return Result.Success;
    }
    public bool CanSell()
    {
        return IsActive && UserProfile is not null;
    }
    public void ChangeRating(double rating)
    {
        AverageRating = rating;
    }
    public void ChangeReviewsCount(int count)
    {
        ReviewCount = count;
    }
    private User() { }

}
