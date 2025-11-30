using System.Security.Cryptography;
using ErrorOr;
using InnoShop.SharedKernel.Common;
using InnoShop.UserManagement.Domain.Common.Interfaces;
using InnoShop.UserManagement.Domain.UserAggregate.Events;

namespace InnoShop.UserManagement.Domain.UserAggregate;

public sealed class User : AggregateRoot
{
    private readonly List<Role> _roles = new();

    private string _passwordHash = null!;


    private User(
        Email email,
        string passwordHash,
        Guid? id = null)
        : base(id ?? Guid.NewGuid())
    {
        Email = email;
        _passwordHash = passwordHash;
        IsEmailVerified = false;

        EmailVerificationToken = Guid.NewGuid().ToString();
        EmailVerificationTokenExpiration = DateTime.UtcNow.AddDays(1);
    }

    
    private User()
    {
    }

    public Email Email { get; } = null!;
    public IReadOnlyCollection<Role> Roles => _roles.ToList();
    public UserProfile? UserProfile { get; private set; }

    public bool IsActive { get; private set; } = true;

    public bool IsEmailVerified { get; private set; }
    public string? EmailVerificationToken { get; private set; }
    public DateTime? EmailVerificationTokenExpiration { get; private set; }

    public string? PasswordResetToken { get; private set; }
    public DateTime? PasswordResetTokenExpiration { get; private set; }


    public RatingSummary RatingSummary { get; private set; } = RatingSummary.Empty;
    public double AverageRating => RatingSummary.Average;
    public int ReviewCount => RatingSummary.NumberOfReviews;

    public ErrorOr<Success> VerifyEmail(string token)
    {
        if (IsEmailVerified) return Error.Conflict(description: "Email already verified");

        if (EmailVerificationToken != token) return Error.Validation(description: "Invalid token");

        if (DateTime.UtcNow > EmailVerificationTokenExpiration)
            return Error.Validation(description: "Token expired");

        _roles.Add(Role.Verified);
        IsEmailVerified = true;
        EmailVerificationToken = null;
        EmailVerificationTokenExpiration = null;

        DomainEvents.Add(new UserEmailVerifiedEvent(Id));

        return Result.Success;
    }

    public void RequestPasswordReset()
    {
        var tokenBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(tokenBytes);
        PasswordResetToken = Convert.ToBase64String(tokenBytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
        PasswordResetTokenExpiration = DateTime.UtcNow.AddHours(1);

        DomainEvents.Add(new PasswordResetRequestedEvent(Id, Email.Value, PasswordResetToken));
    }

    public ErrorOr<Success> ResetPassword(string token, string newPassword, IPasswordHasher passwordHasher)
    {
        if (PasswordResetToken != token)
            return Error.Validation("User.InvalidResetToken", "Invalid or expired password reset token.");
        if (DateTime.UtcNow > PasswordResetTokenExpiration)
            return Error.Validation("User.ExpiredResetToken", "The password reset token has expired.");
        var hashResult = passwordHasher.HashPassword(newPassword);
        if (hashResult.IsError) return hashResult.Errors;
        _passwordHash = hashResult.Value;
        PasswordResetToken = null;
        PasswordResetTokenExpiration = null;
        return Result.Success;
    }

    public bool IsCorrectPasswordHash(string password, IPasswordHasher passwordHasher)
    {
        return passwordHasher.IsCorrectPassword(password, _passwordHash);
    }

    public static User CreateUser(Email email, string passwordHash)
    {
        var user = new User(email, passwordHash);

        user.DomainEvents.Add(new UserRegisteredEvent(user.Id));

        user._roles.Add(Role.Registered);

        return user;
    }

    public static User CreateAdmin(Email email, string passwordHash)
    {
        var user = new User(email, passwordHash);

        user.DomainEvents.Add(new UserRegisteredEvent(user.Id));

        user._roles.Add(Role.Admin);

        return user;
    }

    public ErrorOr<Success> CreateUserProfile(UserProfile userProfile)
    {
        if (UserProfile is not null) return UserErrors.CannotCreateMoreThanOneProfile;

        UserProfile = userProfile;

        DomainEvents.Add(new UserProfileUpdatedEvent(this));

        _roles.Add(Role.Seller);

        return Result.Success;
    }

    public ErrorOr<Success> UpdateUserProfile(UserProfile updatedUserProfile)
    {
        if (UserProfile is null) return UserErrors.UserMustCreateAUserProfile;

        UserProfile = updatedUserProfile;

        DomainEvents.Add(new UserProfileUpdatedEvent(this));

        return Result.Success;
    }

    public ErrorOr<Success> ActivateUser()
    {
        if (IsActive) return UserErrors.UserAlreadyActivated;
        IsActive = true;

        // _domainEvents.Add(new UserActivated(Id));
        return Result.Success;
    }

    public ErrorOr<Success> DeactivateUser()
    {
        if (!IsActive) return UserErrors.UserAlreadyDeactivated;
        IsActive = false;

        DomainEvents.Add(new UserProfileDeactivatedEvent(Id));
        return Result.Success;
    }

    // Может тут проверять на UserProfile is not null это же бизнес логика
    public ErrorOr<Success> ApplyNewRating(int ratingValue)
    {
        RatingSummary = RatingSummary.AddRating(ratingValue);
        return Result.Success;
    }

    public ErrorOr<Success> ApplyRatingRemoval(int ratingValue)
    {
        if (RatingSummary.NumberOfReviews == 0) return UserErrors.RatingMismatch;
        RatingSummary = RatingSummary.RemoveRating(ratingValue);

        return Result.Success;
    }

    public ErrorOr<Success> ApplyRatingUpdate(int oldRating, int newRating)
    {
        if (RatingSummary.NumberOfReviews == 0) return UserErrors.RatingMismatch;

        RatingSummary = RatingSummary
            .RemoveRating(oldRating)
            .AddRating(newRating);

        return Result.Success;
    }

    public bool CanSell()
    {
        return IsActive && UserProfile is not null;
    }
}