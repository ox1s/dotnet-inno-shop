using ErrorOr;

namespace UserManagement.Domain;

public class User
{
    private readonly Guid _id;
    private bool _hasProfile;

    public Guid Id { get; }
    public bool HasProfile { get; }

    public User(Guid? id = null)
    {
        _id = id ?? Guid.NewGuid();
        _hasProfile = false;
    }

    public ErrorOr<UserProfile> CreateProfile()
    {
        if (_hasProfile)
        {
            return UserProfileErrors.CannotHaveMoreProfilesThanOneForUser;
        }

        _hasProfile = true;

        var userProfile = new UserProfile(this.Id);

        return userProfile;
    }
}