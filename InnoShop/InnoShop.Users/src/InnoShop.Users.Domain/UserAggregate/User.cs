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
        bool isActive,
        string passwordHash,
        Role role,
        Guid? id = null)
            : base(id ?? Guid.NewGuid())
    {
        Email = email;
        IsActive = isActive;
        Role = role;
        _passwordHash = passwordHash;
    }

    public bool IsCorrectPasswordHash(string password, IPasswordHasher passwordHasher)
    {
        return passwordHasher.IsCorrectPassword(password, _passwordHash);
    }

    public static User CreateUser(
        Email email,
        bool isActive,
        string passwordHash)
    {
        return new(email, isActive, passwordHash, Role.User);
    }


    public static User CreateAdmin(
        Email email,
        bool isActive,
        string passwordHash)
    {
        return new(email, isActive, passwordHash, Role.Admin);
    }

    public ErrorOr<Success> CreateUserProfile(
       FirstName firstName,
       LastName lastName,
       AvatarUrl avatarUrl,
       PhoneNumber phoneNumber,
       Location location)
    {
        if (UserProfile is not null)
        {
            return Error.Conflict(description: "User already has a profile");
        }

        var userProfile = new UserProfile(
            firstName: firstName,
            lastName: lastName,
            avatarUrl: avatarUrl,
            phoneNumber: phoneNumber,
            location: location
        );

        UserProfile = userProfile;


        //_domainEvents.Add(new UserProfileCreatedEvent(this.Id, userProfile.Id));

        return Result.Success;
    }

    public ErrorOr<Success> CreateReview()
    {
        var review = new Review(userId: Id);
        if (_reservations.Any(existingReservation => existingReservation.ParticipantId == reservation.ParticipantId))
        {
            return SessionErrors.ParticipantCannotReserveTwice;
        }

        _reservations.Add(reservation);
        _domainEvents.Add(new SessionSpotReservedEvent(this, reservation));

        return Result.Success;
    }


    private User() { }



}
